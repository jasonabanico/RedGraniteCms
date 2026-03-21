using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.GraphQl;
using RedGraniteCms.Server.Services;

namespace RedGraniteCms.Server.IntegrationTests;

/// <summary>
/// Integration tests for Item GraphQL queries.
/// </summary>
public class ItemQueryIntegrationTests
{
    private async Task<IRequestExecutor> CreateExecutorAsync(IItemRepository? mockRepository = null)
    {
        var services = new ServiceCollection();

        // Register mock repository if provided
        if (mockRepository != null)
        {
            services.AddSingleton(mockRepository);
        }
        else
        {
            services.AddSingleton<IItemRepository, InMemoryItemRepository>();
        }

        services.AddScoped<IItemService, ItemService>();
        services.AddLogging();
        services.AddGraphQl();

        var serviceProvider = services.BuildServiceProvider();
        var executor = await serviceProvider.GetRequiredService<IRequestExecutorResolver>()
            .GetRequestExecutorAsync();

        return executor;
    }

    [Fact]
    public async Task GetItem_WithValidId_ReturnsItem()
    {
        // Arrange
        var repository = new InMemoryItemRepository();
        var item = Item.Create(ownerId: "user-1", contentType: "blog-post", title: "Test Item", summary: "Short Desc", content: "Long Description");
        await repository.AddItemAsync(item);

        var executor = await CreateExecutorAsync(repository);

        // Act
        var result = await executor.ExecuteAsync($@"
            query {{
                GetItem(id: ""{item.Id}"") {{
                    id
                    title
                    summary
                    content
                }}
            }}
        ");

        // Assert
        result.ExpectQueryResult();
        var data = result.ToJson();
        data.Should().Contain("Test Item");
        data.Should().Contain("Short Desc");
    }

    [Fact]
    public async Task GetItem_WithInvalidId_ReturnsError()
    {
        // Arrange
        var executor = await CreateExecutorAsync(new InMemoryItemRepository());
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await executor.ExecuteAsync($@"
            query {{
                GetItem(id: ""{nonExistentId}"") {{
                    id
                    title
                }}
            }}
        ");

        // Assert
        result.ExpectQueryResult();
        var queryResult = result as IQueryResult;
        queryResult?.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetItems_ReturnsAllItems()
    {
        // Arrange
        var repository = new InMemoryItemRepository();
        await repository.AddItemAsync(Item.Create(ownerId: "user-1", contentType: "blog-post", title: "Item 1", summary: "Short 1", content: "Long 1"));
        await repository.AddItemAsync(Item.Create(ownerId: "user-1", contentType: "blog-post", title: "Item 2", summary: "Short 2", content: "Long 2"));

        var executor = await CreateExecutorAsync(repository);

        // Act
        var result = await executor.ExecuteAsync(@"
            query {
                GetItems(count: 10) {
                    id
                    title
                }
            }
        ");

        // Assert
        result.ExpectQueryResult();
        var data = result.ToJson();
        data.Should().Contain("Item 1");
        data.Should().Contain("Item 2");
    }

    [Fact]
    public async Task GetItems_WithStatusFilter_ReturnsFilteredItems()
    {
        // Arrange
        var repository = new InMemoryItemRepository();
        var published = Item.Create(ownerId: "user-1", contentType: "blog-post", title: "Published Item",
            status: ItemStatus.Published, visibility: ItemVisibility.Public);
        var draft = Item.Create(ownerId: "user-1", contentType: "blog-post", title: "Draft Item");
        await repository.AddItemAsync(published);
        await repository.AddItemAsync(draft);

        var executor = await CreateExecutorAsync(repository);

        // Act
        var result = await executor.ExecuteAsync(@"
            query {
                GetItems(status: PUBLISHED, visibility: PUBLIC, count: 10) {
                    id
                    title
                }
            }
        ");

        // Assert
        result.ExpectQueryResult();
        var data = result.ToJson();
        data.Should().Contain("Published Item");
        data.Should().NotContain("Draft Item");
    }
}

/// <summary>
/// In-memory implementation of IItemRepository for testing.
/// </summary>
public class InMemoryItemRepository : IItemRepository
{
    private readonly List<Item> _items = new();

    public Task<Item?> GetItemAsync(Guid? id = null, string? slug = null, ItemStatus? status = null, ItemVisibility? visibility = null)
    {
        var query = _items.AsEnumerable();

        if (id.HasValue)
            query = query.Where(i => i.Id == id.Value);

        if (slug is not null)
            query = query.Where(i => i.Slug == slug);

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (visibility.HasValue)
            query = query.Where(i => i.Visibility == visibility.Value);

        return Task.FromResult(query.FirstOrDefault());
    }

    public Task<List<Item>> GetItemsAsync(ItemStatus? status = null, ItemVisibility? visibility = null, string? contentType = null, DateTimeOffset? maxDate = null, int? count = null, int skip = 0)
    {
        var effectiveCount = count ?? 50;
        var query = _items.AsEnumerable();

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (visibility.HasValue)
            query = query.Where(i => i.Visibility == visibility.Value);

        if (contentType is not null)
            query = query.Where(i => i.ContentType == contentType);

        if (maxDate.HasValue)
            query = query.Where(i => i.LastModifiedAt < maxDate.Value);

        if (status == ItemStatus.Published)
        {
            query = query
                .OrderBy(i => i.SortOrder)
                .ThenByDescending(i => i.PublishedAt);
        }
        else
        {
            query = query.OrderByDescending(i => i.LastModifiedAt);
        }

        var items = query.Skip(skip).Take(effectiveCount).ToList();

        return Task.FromResult(items);
    }

    public Task<Item?> AddItemAsync(Item item)
    {
        _items.Add(item);
        return Task.FromResult<Item?>(item);
    }

    public Task<Item?> UpdateItemAsync(Guid id, Item item)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem != null)
        {
            existingItem.Update(
                title: item.Title,
                summary: item.Summary,
                content: item.Content,
                status: item.Status,
                visibility: item.Visibility,
                language: item.Language,
                slug: item.Slug,
                contentType: item.ContentType,
                parentId: item.ParentId,
                ancestorIds: item.AncestorIds,
                sortOrder: item.SortOrder,
                metadata: item.Metadata,
                tags: item.Tags);
        }
        return Task.FromResult(existingItem);
    }

    public Task DeleteItemAsync(Guid id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            _items.Remove(item);
        }
        return Task.CompletedTask;
    }
}

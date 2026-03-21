using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.GraphQl.Errors;
using RedGraniteCms.Server.GraphQl.Types;
using RedGraniteCms.Server.GraphQl.Validators;
using RedGraniteCms.Server.Services;

namespace RedGraniteCms.Server.IntegrationTests;

/// <summary>
/// Integration tests for Item mutations via direct service invocation.
/// These tests verify the business logic and validation without the complexity
/// of [UseServiceScope] in GraphQL mutations.
/// </summary>
public class ItemMutationIntegrationTests
{
    private (IItemService service, InMemoryItemRepository repository) CreateServices()
    {
        var repository = new InMemoryItemRepository();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var service = new ItemService(repository, loggerFactory.CreateLogger<ItemService>());
        return (service, repository);
    }

    [Fact]
    public async Task AddItem_WithValidInput_CreatesItem()
    {
        // Arrange
        var (service, repository) = CreateServices();
        var input = new ItemInput
        {
            OwnerId = "user-1",
            ContentType = "blog-post",
            Title = "New Item",
            Summary = "Short Description",
            Content = "This is a long description for the new item"
        };

        // Validate input
        var validator = new ItemInputValidator();
        var validationResult = await validator.ValidateAsync(input);
        validationResult.IsValid.Should().BeTrue();

        // Act
        var newItem = Item.Create(
            ownerId: input.OwnerId,
            contentType: input.ContentType,
            title: input.Title,
            summary: input.Summary,
            content: input.Content);
        var result = await service.AddItemAsync(newItem);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("New Item");
        result.Summary.Should().Be("Short Description");

        // Verify item was added to repository
        var items = await repository.GetItemsAsync(count: 10);
        items.Should().HaveCount(1);
        items[0].Title.Should().Be("New Item");
    }

    [Fact]
    public async Task AddItem_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var input = new ItemInput
        {
            OwnerId = "user-1",
            ContentType = "blog-post",
            Title = "",
            Summary = "Short",
            Content = "Long"
        };

        // Act
        var validator = new ItemInputValidator();
        var validationResult = await validator.ValidateAsync(input);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task UpdateItem_WithValidInput_UpdatesItem()
    {
        // Arrange
        var (service, repository) = CreateServices();
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Original Name",
            summary: "Original Short",
            content: "Original Long");
        await repository.AddItemAsync(item);

        var input = new ItemInput
        {
            Id = item.Id.ToString(),
            OwnerId = "user-1",
            ContentType = "blog-post",
            Title = "Updated Name",
            Summary = "Updated Short",
            Content = "Updated Long Description"
        };

        // Validate input
        var validator = new ItemInputUpdateValidator();
        var validationResult = await validator.ValidateAsync(input);
        validationResult.IsValid.Should().BeTrue();

        // Act
        var updatedItem = Item.Create(
            ownerId: input.OwnerId,
            contentType: input.ContentType,
            title: input.Title,
            summary: input.Summary,
            content: input.Content);
        var result = await service.UpdateItemAsync(item.Id, updatedItem);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Name");

        // Verify item was updated in repository
        var storedItem = await repository.GetItemAsync(id: item.Id);
        storedItem?.Title.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateItem_WithEmptyId_FailsValidation()
    {
        // Arrange
        var input = new ItemInput
        {
            Id = "",
            OwnerId = "user-1",
            ContentType = "blog-post",
            Title = "Updated Name",
            Summary = "Updated Short",
            Content = "Updated Long Description"
        };

        // Act
        var validator = new ItemInputUpdateValidator();
        var validationResult = await validator.ValidateAsync(input);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task DeleteItem_WithValidId_DeletesItem()
    {
        // Arrange
        var (service, repository) = CreateServices();
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "To Delete",
            summary: "Short",
            content: "Long");
        await repository.AddItemAsync(item);

        // Verify item exists
        var existingItems = await repository.GetItemsAsync(count: 10);
        existingItems.Should().HaveCount(1);

        // Act
        await service.DeleteItemAsync(item.Id);

        // Assert - Verify item was deleted from repository
        var items = await repository.GetItemsAsync(count: 10);
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteItem_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var (service, _) = CreateServices();

        // Act & Assert
        var action = () => service.DeleteItemAsync(Guid.NewGuid());
        await action.Should().ThrowAsync<RedGraniteCms.Server.Core.Exceptions.NotFoundException>();
    }
}

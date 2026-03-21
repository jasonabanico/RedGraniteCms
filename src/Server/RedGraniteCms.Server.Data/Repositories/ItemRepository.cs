using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(AppDbContext dbContext, ILogger<ItemRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Item?> GetItemAsync(Guid? id = null, string? slug = null, ItemStatus? status = null, ItemVisibility? visibility = null)
    {
        _logger.LogDebug("Querying item with id: {Id}, slug: {Slug}, status: {Status}, visibility: {Visibility}", id, slug, status, visibility);

        var query = _dbContext.Items.AsNoTracking().AsQueryable();

        if (id.HasValue)
            query = query.Where(i => i.Id == id.Value);

        if (slug is not null)
            query = query.Where(i => i.Slug == slug);

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (visibility.HasValue)
            query = query.Where(i => i.Visibility == visibility.Value);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<Item>> GetItemsAsync(ItemStatus? status = null, ItemVisibility? visibility = null, string? contentType = null, DateTimeOffset? maxDate = null, int? count = null, int skip = 0)
    {
        const int maxAllowedCount = 500;
        const int defaultCount = 50;

        if (count > maxAllowedCount)
        {
            _logger.LogWarning("Requested count {Count} exceeds maximum of {MaxCount}", count, maxAllowedCount);
            throw new ArgumentException($"Item limit is {maxAllowedCount}", nameof(count));
        }

        var effectiveCount = count ?? defaultCount;

        _logger.LogDebug("Querying items with status: {Status}, visibility: {Visibility}, contentType: {ContentType}, maxDate: {MaxDate}, count: {Count}, skip: {Skip}",
            status, visibility, contentType, maxDate, effectiveCount, skip);

        var query = _dbContext.Items.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);

        if (visibility.HasValue)
            query = query.Where(i => i.Visibility == visibility.Value);

        if (contentType is not null)
            query = query.Where(i => i.ContentType == contentType);

        if (maxDate.HasValue)
            query = query.Where(i => i.LastModifiedAt < maxDate.Value);

        // Published items sort by sort order then publish date; otherwise by last modified
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

        var items = await query
            .Skip(skip)
            .Take(effectiveCount)
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} items from database", items.Count);
        return items;
    }

    public async Task<Item?> AddItemAsync(Item item)
    {
        _logger.LogDebug("Adding item to database: {ItemTitle}", item.Title);
        _dbContext.Add(item);
        await _dbContext.SaveChangesAsync();
        _logger.LogDebug("Item added with ID: {ItemId}", item.Id);
        return item;
    }

    public async Task<Item?> UpdateItemAsync(Guid id, Item item)
    {
        _logger.LogDebug("Updating item in database: {ItemId}", id);
        var savedItem = await _dbContext.Items.FindAsync(id);
        if (savedItem != null)
        {
            savedItem.Update(
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
                tags: item.Tags
            );
            _dbContext.Update(savedItem);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Item updated: {ItemId}", id);
        }
        return savedItem;
    }

    public async Task DeleteItemAsync(Guid id)
    {
        _logger.LogDebug("Deleting item from database: {ItemId}", id);
        var savedItem = await _dbContext.Items.FindAsync(id);
        if (savedItem != null)
        {
            _dbContext.Remove(savedItem);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Item deleted: {ItemId}", id);
        }
    }
}

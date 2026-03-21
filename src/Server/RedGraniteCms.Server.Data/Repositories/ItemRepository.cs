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

    public async Task<Item?> GetItemAsync(Guid id)
    {
        _logger.LogDebug("Querying item with ID: {ItemId}", id);
        return await _dbContext.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Item?> GetItemBySlugAsync(string slug)
    {
        _logger.LogDebug("Querying item with slug: {Slug}", slug);
        return await _dbContext.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(item =>
                item.Slug == slug
                && item.Status == ItemStatus.Published
                && item.Visibility == ItemVisibility.Public);
    }

    public async Task<List<Item>> GetItemsAsync(DateTimeOffset? maxDate, int? count, int skip = 0)
    {
        const int maxAllowedCount = 500;
        const int defaultCount = 50;

        if (count > maxAllowedCount)
        {
            _logger.LogWarning("Requested count {Count} exceeds maximum of {MaxCount}", count, maxAllowedCount);
            throw new ArgumentException($"Item limit is {maxAllowedCount}", nameof(count));
        }

        var effectiveMaxDate = maxDate ?? new DateTimeOffset(2999, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var effectiveCount = count ?? defaultCount;

        _logger.LogDebug("Querying items with maxDate: {MaxDate}, count: {Count}, skip: {Skip}", effectiveMaxDate, effectiveCount, skip);

        // Filter in database rather than loading all items into memory
        var items = await _dbContext.Items
            .AsNoTracking()
            .Where(i => i.LastModifiedAt < effectiveMaxDate)
            .OrderByDescending(e => e.LastModifiedAt)
            .Skip(skip)
            .Take(effectiveCount)
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} items from database", items.Count);
        return items;
    }

    public async Task<List<Item>> GetPublishedItemsAsync(int? count, int skip = 0)
    {
        const int defaultCount = 50;
        var effectiveCount = count ?? defaultCount;

        _logger.LogDebug("Querying published items with count: {Count}, skip: {Skip}", effectiveCount, skip);

        var items = await _dbContext.Items
            .AsNoTracking()
            .Where(i => i.Status == ItemStatus.Published
                     && i.Visibility == ItemVisibility.Public)
            .OrderBy(i => i.SortOrder)
            .ThenByDescending(i => i.PublishedAt)
            .Skip(skip)
            .Take(effectiveCount)
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} published items from database", items.Count);
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

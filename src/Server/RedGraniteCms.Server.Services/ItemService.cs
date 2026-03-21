using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Exceptions;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository itemRepository, ILogger<ItemService> logger)
    {
        _repository = itemRepository;
        _logger = logger;
    }

    public async Task<Item?> GetItemAsync(Guid? id = null, string? slug = null, ItemStatus? status = null, ItemVisibility? visibility = null)
    {
        _logger.LogDebug("Getting item with id: {Id}, slug: {Slug}, status: {Status}, visibility: {Visibility}", id, slug, status, visibility);

        if (id is null && slug is null)
            throw new ArgumentException("Either id or slug must be provided.");

        var item = await _repository.GetItemAsync(id, slug, status, visibility);

        if (item is null)
        {
            var identifier = id?.ToString() ?? slug ?? "unknown";
            _logger.LogWarning("Item not found: {Identifier}", identifier);
            throw new NotFoundException(nameof(Item), identifier);
        }

        return item;
    }

    public async Task<List<Item>> GetItemsAsync(ItemStatus? status = null, ItemVisibility? visibility = null, string? contentType = null, DateTimeOffset? maxDate = null, int? count = null, int skip = 0)
    {
        _logger.LogDebug("Getting items with status: {Status}, visibility: {Visibility}, contentType: {ContentType}, maxDate: {MaxDate}, count: {Count}, skip: {Skip}",
            status, visibility, contentType, maxDate, count, skip);

        var items = await _repository.GetItemsAsync(status, visibility, contentType, maxDate, count, skip);

        _logger.LogDebug("Retrieved {Count} items", items.Count);
        return items;
    }

    public async Task<Item?> AddItemAsync(Item item)
    {
        _logger.LogInformation("Adding new item: {ItemTitle}", item.Title);

        var result = await _repository.AddItemAsync(item);

        _logger.LogInformation("Successfully added item with ID: {ItemId}", result?.Id);
        return result;
    }

    public async Task<Item?> UpdateItemAsync(Guid id, Item item)
    {
        _logger.LogInformation("Updating item: {ItemId}", id);

        var existingItem = await _repository.GetItemAsync(id: id);
        if (existingItem is null)
        {
            _logger.LogWarning("Cannot update - item not found: {ItemId}", id);
            throw new NotFoundException(nameof(Item), id.ToString());
        }

        var result = await _repository.UpdateItemAsync(id, item);

        _logger.LogInformation("Successfully updated item: {ItemId}", id);
        return result;
    }

    public async Task DeleteItemAsync(Guid id)
    {
        _logger.LogInformation("Deleting item: {ItemId}", id);

        var existingItem = await _repository.GetItemAsync(id: id);
        if (existingItem is null)
        {
            _logger.LogWarning("Cannot delete - item not found: {ItemId}", id);
            throw new NotFoundException(nameof(Item), id.ToString());
        }

        await _repository.DeleteItemAsync(id);

        _logger.LogInformation("Successfully deleted item: {ItemId}", id);
    }
}

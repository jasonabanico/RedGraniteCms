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

    public async Task<Item?> GetItemAsync(Guid id)
    {
        _logger.LogDebug("Getting item with ID: {ItemId}", id);
        
        var item = await _repository.GetItemAsync(id);
        
        if (item is null)
        {
            _logger.LogWarning("Item not found: {ItemId}", id);
            throw new NotFoundException(nameof(Item), id.ToString());
        }
        
        return item;
    }

    public async Task<Item?> GetItemBySlugAsync(string slug)
    {
        _logger.LogDebug("Getting item with slug: {Slug}", slug);

        var item = await _repository.GetItemBySlugAsync(slug);

        if (item is null)
        {
            _logger.LogWarning("Item not found for slug: {Slug}", slug);
            throw new NotFoundException(nameof(Item), slug);
        }

        return item;
    }

    public async Task<List<Item>> GetItemsAsync(DateTimeOffset? maxDate, int? count, int skip = 0)
    {
        _logger.LogDebug("Getting items with maxDate: {MaxDate}, count: {Count}, skip: {Skip}", maxDate, count, skip);
        
        var items = await _repository.GetItemsAsync(maxDate, count, skip);
        
        _logger.LogDebug("Retrieved {Count} items", items.Count);
        return items;
    }

    public async Task<List<Item>> GetPublishedItemsAsync(int? count, int skip = 0)
    {
        _logger.LogDebug("Getting published items with count: {Count}, skip: {Skip}", count, skip);

        var items = await _repository.GetPublishedItemsAsync(count, skip);

        _logger.LogDebug("Retrieved {Count} published items", items.Count);
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
        
        var existingItem = await _repository.GetItemAsync(id);
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
        
        var existingItem = await _repository.GetItemAsync(id);
        if (existingItem is null)
        {
            _logger.LogWarning("Cannot delete - item not found: {ItemId}", id);
            throw new NotFoundException(nameof(Item), id.ToString());
        }
        
        await _repository.DeleteItemAsync(id);
        
        _logger.LogInformation("Successfully deleted item: {ItemId}", id);
    }
}

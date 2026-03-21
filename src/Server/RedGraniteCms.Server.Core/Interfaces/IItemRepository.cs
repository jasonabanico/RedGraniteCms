using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Core.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetItemAsync(Guid? id = null, string? slug = null, ItemStatus? status = null, ItemVisibility? visibility = null);
    Task<List<Item>> GetItemsAsync(ItemStatus? status = null, ItemVisibility? visibility = null, string? contentType = null, DateTimeOffset? maxDate = null, int? count = null, int skip = 0);
    Task<Item?> AddItemAsync(Item item);
    Task<Item?> UpdateItemAsync(Guid id, Item item);
    Task DeleteItemAsync(Guid id);
}

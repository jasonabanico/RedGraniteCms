using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Core.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetItemAsync(Guid id);
    Task<List<Item>> GetItemsAsync(DateTimeOffset? maxDate, int? count);
    Task<Item?> AddItemAsync(Item item);
    Task<Item?> UpdateItemAsync(Guid id, Item item);
    Task DeleteItemAsync(Guid id);
}

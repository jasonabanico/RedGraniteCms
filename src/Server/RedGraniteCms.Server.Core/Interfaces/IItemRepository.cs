using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Core.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetItemAsync(Guid id);
    Task<Item?> GetItemBySlugAsync(string slug);
    Task<List<Item>> GetItemsAsync(DateTimeOffset? maxDate, int? count, int skip = 0);
    Task<List<Item>> GetPublishedItemsAsync(int? count, int skip = 0);
    Task<Item?> AddItemAsync(Item item);
    Task<Item?> UpdateItemAsync(Guid id, Item item);
    Task DeleteItemAsync(Guid id);
}

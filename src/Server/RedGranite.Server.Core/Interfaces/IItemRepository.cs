using RedGranite.Server.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace RedGranite.Server.Core.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetItemAsync(string id);
    Task<List<Item>> GetItemsAsync(DateTimeOffset? maxDate, int? count);
    Task<Item?> AddItemAsync(Item item);
    Task<Item?> UpdateItemAsync(string id, Item item);
    Task DeleteItemAsync(string id);
}

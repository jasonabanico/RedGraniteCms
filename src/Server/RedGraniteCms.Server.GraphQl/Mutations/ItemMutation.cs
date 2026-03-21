using FluentValidation;
using HotChocolate.Authorization;
using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.GraphQl.Types;
using RedGraniteCms.Server.GraphQl.Validators;
using System.Security.Claims;
using System.Text.Json.Nodes;
using ValidationException = RedGraniteCms.Server.Core.Exceptions.ValidationException;

namespace RedGraniteCms.Server.GraphQl.Mutations;

/// <summary>
/// GraphQL mutations for Item operations.
/// All mutations require authentication.
/// </summary>
[Authorize]
public class ItemMutation
{
    private readonly ILogger<ItemMutation> _logger;

    public ItemMutation(ILogger<ItemMutation> logger)
    {
        _logger = logger;
    }

    [UseServiceScope]
    [GraphQLName("AddItem")]
    public async Task<Item?> AddItemAsync(
        ItemInput item,
        ClaimsPrincipal claimsPrincipal,
        [Service] IItemService itemService)
    {
        var userId = GetUserId(claimsPrincipal);
        _logger.LogDebug("AddItem mutation called by {UserId} for: {ItemTitle}", userId, item.Title);

        // Validate input
        var validator = new ItemInputValidator();
        var validationResult = await validator.ValidateAsync(item);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ValidationException(errors);
        }

        var status = Enum.TryParse<ItemStatus>(item.Status, true, out var s) ? s : ItemStatus.Draft;
        var visibility = Enum.TryParse<ItemVisibility>(item.Visibility, true, out var v) ? v : ItemVisibility.Private;
        JsonObject? metadata = item.MetadataJson is not null ? JsonNode.Parse(item.MetadataJson)?.AsObject() : null;

        var newItem = Item.Create(
            ownerId: userId,
            contentType: item.ContentType,
            title: item.Title,
            summary: item.Summary,
            content: item.Content,
            status: status,
            visibility: visibility,
            language: item.Language ?? "en",
            slug: item.Slug,
            parentId: item.ParentId,
            ancestorIds: item.AncestorIds,
            sortOrder: item.SortOrder ?? 0,
            metadata: metadata,
            tags: item.Tags,
            createdBy: userId
        );

        return await itemService.AddItemAsync(newItem);
    }

    [UseServiceScope]
    [GraphQLName("UpdateItem")]
    public async Task<Item?> UpdateItemAsync(
        ItemInput item,
        ClaimsPrincipal claimsPrincipal,
        [Service] IItemService itemService)
    {
        var userId = GetUserId(claimsPrincipal);
        _logger.LogDebug("UpdateItem mutation called by {UserId} for ID: {ItemId}", userId, item.Id);

        // Validate input with ID requirement
        var validator = new ItemInputUpdateValidator();
        var validationResult = await validator.ValidateAsync(item);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ValidationException(errors);
        }

        var status = Enum.TryParse<ItemStatus>(item.Status, true, out var s) ? s : ItemStatus.Draft;
        var visibility = Enum.TryParse<ItemVisibility>(item.Visibility, true, out var v) ? v : ItemVisibility.Private;
        JsonObject? metadata = item.MetadataJson is not null ? JsonNode.Parse(item.MetadataJson)?.AsObject() : null;

        var updatedItem = Item.Create(
            ownerId: userId,
            contentType: item.ContentType,
            title: item.Title,
            summary: item.Summary,
            content: item.Content,
            status: status,
            visibility: visibility,
            language: item.Language ?? "en",
            slug: item.Slug,
            parentId: item.ParentId,
            ancestorIds: item.AncestorIds,
            sortOrder: item.SortOrder ?? 0,
            metadata: metadata,
            tags: item.Tags,
            createdBy: userId
        );

        return await itemService.UpdateItemAsync(Guid.Parse(item.Id!), updatedItem);
    }

    [UseServiceScope]
    [GraphQLName("DeleteItem")]
    public async Task<bool> DeleteItemAsync(
        string id,
        ClaimsPrincipal claimsPrincipal,
        [Service] IItemService itemService)
    {
        var userId = GetUserId(claimsPrincipal);
        _logger.LogDebug("DeleteItem mutation called by {UserId} for ID: {ItemId}", userId, id);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ValidationException("id", "Item ID is required.");
        }

        await itemService.DeleteItemAsync(Guid.Parse(id));
        return true;
    }

    private static string GetUserId(ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID claim not found.");
    }
}

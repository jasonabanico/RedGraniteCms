using HotChocolate.Authorization;
using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.GraphQl.Queries;

/// <summary>
/// GraphQL queries for Item operations.
/// Two query endpoints that accept optional filters for various use cases.
/// </summary>
public class ItemQuery
{
    private readonly ILogger<ItemQuery> _logger;

    public ItemQuery(ILogger<ItemQuery> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns a single item. Provide at least one of id or slug.
    /// Optionally filter by status and/or visibility.
    /// 
    /// Use cases:
    ///   GetItem(id: "...")                                          — admin lookup by ID
    ///   GetItem(slug: "about", status: PUBLISHED, visibility: PUBLIC) — public page by slug
    /// </summary>
    [UseServiceScope]
    [GraphQLName("GetItem")]
    public async Task<Item?> GetItemAsync(
        string? id = null,
        string? slug = null,
        ItemStatus? status = null,
        ItemVisibility? visibility = null,
        [Service] IItemService itemService = null!)
    {
        _logger.LogDebug("GetItem query called with id: {Id}, slug: {Slug}, status: {Status}, visibility: {Visibility}", id, slug, status, visibility);

        Guid? guidId = id is not null ? Guid.Parse(id) : null;
        return await itemService.GetItemAsync(guidId, slug, status, visibility);
    }

    /// <summary>
    /// Returns a list of items with optional filters and pagination.
    /// 
    /// Use cases:
    ///   GetItems(count: 10)                                                    — admin listing
    ///   GetItems(status: PUBLISHED, visibility: PUBLIC, count: 50)             — public pages
    ///   GetItems(contentType: "blog-post", status: PUBLISHED, count: 20)       — blog listing
    ///   GetItems(isoMaxDate: "2025-01-01T00:00:00Z", count: 10, skip: 10)     — cursor pagination
    /// </summary>
    [UseServiceScope]
    [GraphQLName("GetItems")]
    public async Task<List<Item>> GetItemsAsync(
        ItemStatus? status = null,
        ItemVisibility? visibility = null,
        string? contentType = null,
        string? isoMaxDate = null,
        int? count = null,
        int skip = 0,
        [Service] IItemService itemService = null!)
    {
        _logger.LogDebug("GetItems query called with status: {Status}, visibility: {Visibility}, contentType: {ContentType}, maxDate: {MaxDate}, count: {Count}, skip: {Skip}",
            status, visibility, contentType, isoMaxDate, count, skip);

        DateTimeOffset? maxDate = null;
        if (DateTimeOffset.TryParse(isoMaxDate, out var parsed))
            maxDate = parsed;

        return await itemService.GetItemsAsync(status, visibility, contentType, maxDate, count, skip);
    }
}

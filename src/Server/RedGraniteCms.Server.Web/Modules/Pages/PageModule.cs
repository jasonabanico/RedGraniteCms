using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RedGraniteCms.Server.Web.Modules.Pages;

/// <summary>
/// Page module implementation. Communicates with the Api's GraphQL endpoint
/// and maps Item responses to Page models.
/// </summary>
public class PageModule : IPageModule
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PageModule> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public PageModule(HttpClient httpClient, ILogger<PageModule> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Page?> GetPageBySlugAsync(string slug)
    {
        var query = new
        {
            query = @"
                query GetItem($slug: String, $status: ItemStatus, $visibility: ItemVisibility) {
                    GetItem(slug: $slug, status: $status, visibility: $visibility) {
                        id title summary content slug language
                        publishedAt tags metadata
                    }
                }",
            variables = new { slug, status = "PUBLISHED", visibility = "PUBLIC" }
        };

        var dto = await ExecuteQueryAsync<ItemResponse>("GetItem", query);
        return dto is not null ? MapToPage(dto) : null;
    }

    public async Task<List<Page>> GetPublishedPagesAsync(int count = 50, int skip = 0)
    {
        var query = new
        {
            query = @"
                query GetItems($status: ItemStatus, $visibility: ItemVisibility, $count: Int, $skip: Int) {
                    GetItems(status: $status, visibility: $visibility, count: $count, skip: $skip) {
                        id title summary content slug language
                        publishedAt tags metadata
                    }
                }",
            variables = new { status = "PUBLISHED", visibility = "PUBLIC", count, skip }
        };

        var dtos = await ExecuteQueryAsync<List<ItemResponse>>("GetItems", query);
        return dtos?.Select(MapToPage).ToList() ?? [];
    }

    private static Page MapToPage(ItemResponse item)
    {
        JsonObject? metadata = null;
        if (item.Metadata is not null)
        {
            metadata = item.Metadata;
        }

        return new Page
        {
            Id = item.Id,
            Title = item.Title,
            Summary = item.Summary,
            Content = item.Content,
            Slug = item.Slug,
            Language = item.Language,
            PublishedAt = item.PublishedAt,
            Tags = item.Tags ?? [],
            MetaDescription = metadata?["seo"] is JsonObject seo ? seo["description"]?.GetValue<string>() : null,
            OgImageUrl = metadata?["og"] is JsonObject og ? og["image"]?.GetValue<string>() : null,
        };
    }

    private async Task<T?> ExecuteQueryAsync<T>(string operationName, object query)
    {
        _logger.LogDebug("Executing GraphQL query: {Operation}", operationName);

        var content = new StringContent(
            JsonSerializer.Serialize(query, JsonOptions),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("graphql", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("GraphQL request failed with status {StatusCode}: {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"API request failed: {response.StatusCode}");
        }

        var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (root.TryGetProperty("errors", out var errors) && errors.GetArrayLength() > 0)
        {
            var firstError = errors[0].GetProperty("message").GetString();
            _logger.LogWarning("GraphQL error for {Operation}: {Error}", operationName, firstError);

            var extensions = errors[0].TryGetProperty("extensions", out var ext) ? ext : default;
            var code = extensions.ValueKind != JsonValueKind.Undefined
                && extensions.TryGetProperty("code", out var codeProp)
                    ? codeProp.GetString() : null;

            if (code == "NOT_FOUND")
                return default;

            throw new InvalidOperationException($"GraphQL error: {firstError}");
        }

        if (root.TryGetProperty("data", out var data)
            && data.TryGetProperty(operationName, out var result)
            && result.ValueKind != JsonValueKind.Null)
        {
            return JsonSerializer.Deserialize<T>(result.GetRawText(), JsonOptions);
        }

        return default;
    }

    /// <summary>
    /// Internal DTO for deserializing GraphQL Item responses.
    /// Private to the module — never exposed to controllers.
    /// </summary>
    private class ItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public string Language { get; set; } = "en";
        public DateTimeOffset? PublishedAt { get; set; }
        public List<string>? Tags { get; set; }
        public JsonObject? Metadata { get; set; }
    }
}

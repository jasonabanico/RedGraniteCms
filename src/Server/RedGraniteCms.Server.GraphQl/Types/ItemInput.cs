namespace RedGraniteCms.Server.GraphQl.Types;

public class ItemInput
{
    public string? Id { get; set; }
    public string? OwnerId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? Status { get; set; }
    public string? Visibility { get; set; }
    public string? Language { get; set; }
    public string? Slug { get; set; }
    public string? ParentId { get; set; }
    public List<string>? AncestorIds { get; set; }
    public int? SortOrder { get; set; }
    public string? MetadataJson { get; set; }
    public List<string>? Tags { get; set; }
}

using Microsoft.EntityFrameworkCore;
using RedGraniteCms.Server.Core.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RedGraniteCms.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var item = modelBuilder.Entity<Item>();

        item.HasKey(e => e.Id);

        // Value converters for complex types
        var jsonOptions = new JsonSerializerOptions();

        item.Property(e => e.Metadata)
            .HasConversion(
                v => v.ToJsonString(null),
                v => JsonNode.Parse(v, null, default)!.AsObject());

        item.Property(e => e.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v.ToList(), jsonOptions),
                v => (IReadOnlySet<string>)new HashSet<string>(
                    JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase));

        item.Property(e => e.AncestorIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => (IReadOnlyList<string>)(JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>()));
    }
}

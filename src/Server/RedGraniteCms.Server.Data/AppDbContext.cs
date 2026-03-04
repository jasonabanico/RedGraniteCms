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
        item.Property(e => e.Metadata)
            .HasConversion(
                v => v.ToJsonString(),
                v => JsonNode.Parse(v)!.AsObject());

        item.Property(e => e.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v.ToList(), (JsonSerializerOptions?)null),
                v => (IReadOnlySet<string>)new HashSet<string>(
                    JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? [],
                    StringComparer.OrdinalIgnoreCase));

        item.Property(e => e.AncestorIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => (IReadOnlyList<string>)(JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()));
    }
}

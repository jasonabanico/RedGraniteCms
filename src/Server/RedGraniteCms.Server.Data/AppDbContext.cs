using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RedGraniteCms.Server.Core.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RedGraniteCms.Server.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var item = modelBuilder.Entity<Item>();

        item.HasKey(e => e.Id);

        // Indexes
        item.HasIndex(e => e.OwnerId);
        item.HasIndex(e => e.Slug);
        item.HasIndex(e => new { e.OwnerId, e.ContentType });
        item.HasIndex(e => new { e.OwnerId, e.CreatedAt });
        item.HasIndex(e => new { e.Status, e.Visibility });
        item.HasIndex(e => e.LastModifiedAt);

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

        // SQLite does not support DateTimeOffset in ORDER BY.
        // Convert to ISO 8601 strings which sort correctly as text.
        if (Database.IsSqlite())
        {
            var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, string>(
                v => v.ToString("O"),
                v => DateTimeOffset.Parse(v));

            var nullableDateTimeOffsetConverter = new ValueConverter<DateTimeOffset?, string?>(
                v => v.HasValue ? v.Value.ToString("O") : null,
                v => v != null ? DateTimeOffset.Parse(v) : null);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset))
                    {
                        property.SetValueConverter(dateTimeOffsetConverter);
                    }
                    else if (property.ClrType == typeof(DateTimeOffset?))
                    {
                        property.SetValueConverter(nullableDateTimeOffsetConverter);
                    }
                }
            }
        }
    }
}

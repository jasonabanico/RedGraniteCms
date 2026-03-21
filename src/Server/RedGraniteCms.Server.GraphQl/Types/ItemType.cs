using System.Text.Json;
using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.GraphQl.Types;

/// <summary>
/// Configures how HotChocolate maps the Item model to the GraphQL schema.
/// Prevents complex System.Text.Json types from leaking into the schema.
/// </summary>
public class ItemType : ObjectType<Item>
{
    protected override void Configure(IObjectTypeDescriptor<Item> descriptor)
    {
        descriptor.Field(i => i.Metadata)
            .Type<AnyType>()
            .Resolve(ctx =>
            {
                var item = ctx.Parent<Item>();
                // Serialize JsonObject to a dictionary that HotChocolate can handle as JSON
                var json = item.Metadata.ToJsonString();
                return JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
            });

        descriptor.Field(i => i.Tags)
            .Type<ListType<StringType>>()
            .Resolve(ctx =>
            {
                var item = ctx.Parent<Item>();
                return item.Tags.ToList();
            });

        descriptor.Field(i => i.AncestorIds)
            .Type<ListType<StringType>>()
            .Resolve(ctx =>
            {
                var item = ctx.Parent<Item>();
                return item.AncestorIds.ToList();
            });
    }
}

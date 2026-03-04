using Microsoft.Extensions.DependencyInjection;
using RedGraniteCms.Server.GraphQl.Errors;
using RedGraniteCms.Server.GraphQl.Mutations;
using RedGraniteCms.Server.GraphQl.Queries;

namespace RedGraniteCms.Server.GraphQl;

public static class DataExtensions
{
    public static void AddGraphQl(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddErrorFilter<GraphQlErrorFilter>()
            .AddMutationType<ItemMutation>()
            .AddQueryType<ItemQuery>();
    }
}

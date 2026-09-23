namespace SampleApi.Endpoints;

public static class InfrastructureEndpoints
{
    public static IEndpointRouteBuilder AddInfrastructureEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGroup("infrastructure")
            .AddQueuesEndpoints();

        return builder;
    }
}
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspNetApi>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.BlazorWeb>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddViteApp("vite-typescript", "../vite-typescript")
    .WithNpm()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddViteApp("solid-typescript", "../solid-typescript")
    .WithNpm()
    // .WithEnvironment("VITE_APISERVICE_HTTP", apiService.GetEndpoint("http"))
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

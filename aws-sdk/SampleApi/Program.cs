using Amazon.SQS;
using SampleApi.Endpoints;
using SampleApi.Messaging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddMemoryCache()
    .AddLogging()
    .AddSingleton<IAmazonSQS, AmazonSQSClient>()
    .AddSingleton<SQSService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.AddInfrastructureEndpoints();

app.Run();
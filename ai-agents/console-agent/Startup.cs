using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleAgent;

public static class Startup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string argModel)
    {
        services
            .AddLogging(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
            .AddSingleton<ILoggerFactory>(_ =>
                LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information)))
            .AddSingleton<IConfiguration>(_ =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            })
            .AddSingleton(serviceProvider => new DateTimeService(TimeProvider.System))
            .AddSingleton<IChatClient>(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                return new ChatClientBuilder(new OllamaChatClient(new Uri(configuration["Ollama:Endpoint"] ?? string.Empty), GetModel(configuration, argModel)))
                    .UseLogging(loggerFactory)
                    .UseFunctionInvocation(loggerFactory, options => options.IncludeDetailedErrors = true)
                    .Build();
            })
            .AddTransient<ChatOptions>(serviceProvider =>
                new ChatOptions
                {
                    MaxOutputTokens = 1024,
                    Temperature = 1f,
                    ModelId = GetModel(serviceProvider.GetRequiredService<IConfiguration>(), argModel),
                    Tools = [.. FunctionRegistry.GetTools(serviceProvider)]
                });

        return services;
    }

    private static string GetModel(IConfiguration configuration, string argValue)
    {
        return string.IsNullOrWhiteSpace(argValue) ? configuration["Ollama:Model"] ?? string.Empty : argValue;
    }
}

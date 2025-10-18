using ConsoleAgent;
using Microsoft.Extensions.DependencyInjection;

string argModel = string.Empty;

for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--model" && i + 1 < args.Length)
    {
        argModel = args[i + 1];
        break;
    }
}

ServiceProvider serviceProvider = new ServiceCollection().AddApplicationServices(argModel).BuildServiceProvider();
await ChatAgent.RunAsync(serviceProvider);
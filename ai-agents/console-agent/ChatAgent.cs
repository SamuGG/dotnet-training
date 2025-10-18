using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAgent;

public static class ChatAgent
{
    public static async Task RunAsync(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetRequiredService<IChatClient>();
        var chatOptions = serviceProvider.GetRequiredService<ChatOptions>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        string userPrompt = configuration["ChatSettings:UserPrompt"] ?? string.Empty;
        var history = new List<ChatMessage>
        {
            new (ChatRole.System, configuration["ChatSettings:SystemMessage"])
        };

        Console.WriteLine(configuration["ChatSettings:AssistantMessage"]);

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(userPrompt);
            string? userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput)) break;
            Console.ResetColor();

            history.Add(new ChatMessage(ChatRole.User, userInput));

            // ChatResponse response = await client.GetResponseAsync(history, chatOptions);
            // Console.WriteLine(response.Text);
            // history.AddRange(response.Messages);

            string response = string.Empty;
            await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync(history, chatOptions))
            {
                Console.Write(update.Text);
                response += update.Text;
            }

            Console.WriteLine();
            history.AddRange(new ChatMessage(ChatRole.Assistant, response));
        }
    }
}

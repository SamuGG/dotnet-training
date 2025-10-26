using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorConsole;

var services = new ServiceCollection();
services.AddLogging();

IServiceProvider serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

string html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
{
    var componentParams = new Dictionary<string, object?>
    {
        { nameof(MessageComponent.MessageText), "Hello from the Razor component" }
    };

    var parameters = ParameterView.FromDictionary(componentParams);
    HtmlRootComponent output = await htmlRenderer.RenderComponentAsync<MessageComponent>(parameters);

    return output.ToHtmlString();
});

Console.WriteLine(html);
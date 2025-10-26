using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RazorTemplate.Common;
using RazorTemplate.Components;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "To view the template, you can post some sales order data to /sales/template");

app
    .MapGroup("/sales")
    .MapPost("/template/{cultureCode?}", async (IServiceProvider serviceProvider, ILoggerFactory loggerFactory, SalesOrder order, string? cultureCode) =>
    {
        if (order == null)
            return Results.BadRequest();

        CultureInfo requestedCulture = string.IsNullOrWhiteSpace(cultureCode)
            ? CultureInfo.CurrentUICulture
            : new CultureInfo(cultureCode);

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

        string html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var componentParams = new Dictionary<string, object?>
            {
                { nameof(SalesOrderTemplate.Order), order },
                { nameof(SalesOrderTemplate.NumberFormat), requestedCulture.NumberFormat },
                { nameof(SalesOrderTemplate.DateTimeFormat), requestedCulture.DateTimeFormat }
            };

            var parameters = ParameterView.FromDictionary(componentParams);
            var output = await htmlRenderer.RenderComponentAsync<SalesOrderTemplate>(parameters);

            return output.ToHtmlString();
        });

        return Results.Content(html, "text/html");
    });

app.Run();

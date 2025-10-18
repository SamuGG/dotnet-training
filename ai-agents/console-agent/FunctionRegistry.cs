using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAgent;

public static class FunctionRegistry
{
    public static IEnumerable<AITool> GetTools(this IServiceProvider serviceProvider)
    {
        var dateTimeService = serviceProvider.GetRequiredService<DateTimeService>();

        var getLocalDateTimeFn = typeof(DateTimeService).GetMethod(nameof(DateTimeService.GetLocalDateTime)) ?? throw new MissingMethodException(nameof(DateTimeService), nameof(DateTimeService.GetLocalDateTime));

        yield return AIFunctionFactory.Create(getLocalDateTimeFn, dateTimeService, new AIFunctionFactoryOptions
        {
            Name = "GetLocalDateTime",
            Description = "Gets the current local date and time"
        });

        yield return AIFunctionFactory.Create(WardrobeService.ListDayOutfits, new AIFunctionFactoryOptions
        {
            Name = "ListDayOutfits",
            Description = "Gets a list of clothing outfits for the day"
        });

        yield return AIFunctionFactory.Create(WardrobeService.ListEveningOutfits, new AIFunctionFactoryOptions
        {
            Name = "ListEveningOutfits",
            Description = "Gets a list of clothing outfits for the evening"
        });

        yield return AIFunctionFactory.Create(GetCurrentWeather);
    }

    [Description("Get the current weather for a city")]
    private static string GetCurrentWeather(string city)
    {
        return $"The weather in {city} is sunny 24°C with a light breeze from the west at 10 km/h, humidity at 40%, UV index of 5, visibility of 10 km, pressure of 1015 hPa, dew point at 10°C, no precipitation. Expected to remain the same throughout the year.";
    }
}

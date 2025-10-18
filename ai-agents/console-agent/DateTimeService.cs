namespace ConsoleAgent;

public class DateTimeService(TimeProvider timeProvider)
{
    public string GetLocalDateTime()
    {
        return timeProvider.GetLocalNow().ToString("F");
    }
}

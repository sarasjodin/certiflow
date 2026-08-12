namespace CertiFlowApp.Services.DateTime;

public static class ApplicationDateTime
{
    private static readonly TimeZoneInfo StockholmTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    // Converts a UTC timestamp to the application time zone for display.
    // Does not modify the stored db value.
    public static string Format(DateTimeOffset value)
    {
        var localTime = TimeZoneInfo.ConvertTime(
            value,
            StockholmTimeZone);

        return localTime.ToString("yyyy-MM-dd HH:mm");
    }

    // Formats an optional UTC timestamp
    // Handles nullable DateTimeOffset values - returning "-" when no timestamp exists
    public static string Format(DateTimeOffset? value)
    {
        return value.HasValue
            ? Format(value.Value)
            : "-";
    }

    // Formats a calendar date without applying time-zone conversion
    public static string Format(DateOnly? value)
    {
        return value?.ToString("yyyy-MM-dd") ?? "-";
    }

    // Uses TimeProvider to calculate today's calendar date in the application's time zone
    public static DateOnly Today(TimeProvider timeProvider)
    {
        var utcNow = timeProvider.GetUtcNow();

        var localNow = TimeZoneInfo.ConvertTime(
            utcNow,
            StockholmTimeZone);

        return DateOnly.FromDateTime(localNow.DateTime);
    }

    // Converts a UTC timestamp to a calendar date in the application time zone
    public static DateOnly ToDateOnly(DateTimeOffset value)
    {
        var localTime = TimeZoneInfo.ConvertTime(
            value,
            StockholmTimeZone);

        return DateOnly.FromDateTime(localTime.DateTime);
    }
}

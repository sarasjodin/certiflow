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
    // Handles nullable DateTimeOffset values - returning null if the input is null
    public static string? Format(DateTimeOffset? value)
    {
        return value.HasValue
            ? Format(value.Value)
            : null;
    }
}
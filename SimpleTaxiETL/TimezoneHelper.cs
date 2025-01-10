namespace SimpleTaxiETL;

public static class TimezoneHelper
{
    private static TimeZoneInfo? _estTimeZone;

    public static TimeZoneInfo? GetEstTimeZone()
    {
        if (_estTimeZone is not null)
            return _estTimeZone;

        // Names of time zones are system dependent, so we need to try to find the correct one
        try
        {
            _estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            _estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        }

        return _estTimeZone;
    }
}
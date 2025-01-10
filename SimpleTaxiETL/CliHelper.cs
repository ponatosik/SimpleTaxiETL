using System.Globalization;

namespace SimpleTaxiETL;

public static class CliHelper
{
    public static bool ParseFlag(this string[] args, string flag)
    {
        return args.Any(arg => string.Equals(arg, flag, StringComparison.InvariantCultureIgnoreCase));
    }

    public static string? ParseParameter(this string[] args, string parameter)
    {
        return args
            .SkipWhile(arg => !string.Equals(arg, parameter, StringComparison.InvariantCultureIgnoreCase))
            .Skip(1)
            .FirstOrDefault();
    }

    public static T Prompt<T>(string fieldName) where T : IParsable<T>
    {
        Console.WriteLine($"Input {fieldName}:");
        return T.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
    }
}
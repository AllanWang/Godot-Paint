using System;

public static class Helper
{
    public static string OrFallback(this string source, Func<string> fallback)
    {
        return (string.IsNullOrWhiteSpace(source)) ? fallback() : source;
    }
}
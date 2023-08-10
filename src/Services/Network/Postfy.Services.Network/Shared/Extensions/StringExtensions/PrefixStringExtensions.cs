namespace Postfy.Services.Network.Shared.Extensions.StringExtensions;

public static class PrefixStringExtensions
{
    public static string Prefixify(this string current, string prefix)
    {
        return current + prefix;
    }

    public static string Prefixify(this string current)
    {
        return current + "_network";
    }
}

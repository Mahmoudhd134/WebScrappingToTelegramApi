using System.Text;

namespace ScrapingLibrary.Helpers;

public static class StringExtensionMethods
{
    public static string Repeat(this string s, int count)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < count; i++)
            sb.Append(s);
        return sb.ToString();
    }
}
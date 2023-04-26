using System.Collections;
using HtmlAgilityPack;

namespace ScrapingLibrary.Models.WebSites.Sites;

public class KolNovelPages : WebSite<IEnumerable<string>>
{
    public KolNovelPages(string url) : base(url)
    {
    }

    public override async Task<IEnumerable<string>> Parse(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var links = htmlDocument.DocumentNode.Descendants("div")
            .Where(n => n.GetAttributeValue("class", "").Equals("bixbox bxcl epcheck"))
            .SelectMany(d => d.Descendants("li"))
            .SelectMany(li => li.Descendants("a"))
            .Where(a => a.GetAttributeValue("href", "").Equals("") == false)
            .Select(a => a.GetAttributeValue("href", ""))
            .Where(h => string.IsNullOrWhiteSpace(h ?? "") == false).Reverse();
        return links;
    }
}
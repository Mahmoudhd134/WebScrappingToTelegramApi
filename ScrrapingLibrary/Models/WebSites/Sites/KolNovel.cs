using System.Web;
using HtmlAgilityPack;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class KolNovel : WebSite<IEnumerable<string>>
    {
        public KolNovel(string url) : base(url)
        {
        }

        public override Task<IEnumerable<string>> Parse(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var title = GetTitle(htmlDocument);

            var content = htmlDocument.DocumentNode.Descendants("p")
                .Where(p => p.FirstChild.Name.Equals("span"))
                .Select(p => HttpUtility.HtmlDecode(p.Descendants("span").FirstOrDefault()?.InnerText));

            throw new NotImplementedException();
        }

        private string GetTitle(HtmlDocument htmlDocument)
        {
            var entryTitle = htmlDocument.DocumentNode.Descendants("h1")
                .FirstOrDefault(h1 => h1.GetAttributeValue("class", "NO").Equals("entry-title"))?
                .InnerText ?? "";

            var mainTitle = htmlDocument.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.GetAttributeValue("class", "NO").Equals("cat-series"))?
                .InnerText ?? "";

            var anotherTwoTitles = htmlDocument.DocumentNode.Descendants("p")
                .Where(p => p.GetAttributeValue("style", "NO").Equals("text-align: center;") ||
                            p.GetAttributeValue("style", "NO").Equals("text-align: left"))
                .Select(p => p.InnerText)
                .ToList();

            return $"{entryTitle}\n{mainTitle}\n{anotherTwoTitles[0]}\n{anotherTwoTitles[1]}";
        }
    }
}
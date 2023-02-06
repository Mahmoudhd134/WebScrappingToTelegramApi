using HtmlAgilityPack;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class SanFoundryWebSitePages : WebSite<IEnumerable<string>>
    {
        public SanFoundryWebSitePages(string url) : base(url)
        {
        }

        public override Task<IEnumerable<string>> Parse(string html)
        {
            return Task.FromResult(GetPages(html));
        }
    
        private IEnumerable<string> GetPages(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
    
            var div = htmlDoc.DocumentNode
                .SelectNodes("//div[@class='" + "sf-section" + "']");
    
            var currentNumber = 1;
            var hasTable = false;
            foreach (var node in div)
            {
                if (node.InnerText.Trim().StartsWith(currentNumber.ToString()))
                {
                    currentNumber++;
                    var localLinks = node.Descendants("a")
                        .Where(l => l.Attributes.Any(a => a.Name == "href"));
                    foreach (var link in localLinks)
                    {
                        Console.WriteLine("\nLink Found");
                        yield return link.Attributes.First(a => a.Name == "href").Value;
                    }
                }
            }
        }
    }
}
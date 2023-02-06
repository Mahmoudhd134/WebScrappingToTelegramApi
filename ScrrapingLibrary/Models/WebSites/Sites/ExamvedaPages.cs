using HtmlAgilityPack;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class ExamvedaPages : WebSite<IEnumerable<string>>
    {
        public ExamvedaPages(string url) : base(url)
        {
        }

        public override Task<IEnumerable<string>> Parse(string html)
        {
            return Task.FromResult(Extract(html));
        }

        private IEnumerable<string> Extract(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var currentPage = 1;
            return htmlDoc.DocumentNode.Descendants()
                .First(n => n.Name.Equals("div") &&
                            n.GetAttributeValue("class", "").Equals("pagination"))
                .Descendants("a")
                .Where(l => l.GetAttributeValue("href", "").Length > 10 &&
                            l.Id.Equals($"pg_{++currentPage}"))
                .Select(l => l.GetAttributeValue("href", ""));
        }
    }
}
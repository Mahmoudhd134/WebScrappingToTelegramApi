using HtmlAgilityPack;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class Ashq : WebSite<IEnumerable<IEnumerable<string>>>
    {
        public Ashq(string url) : base(url)
        {
        }

        public override Task<IEnumerable<IEnumerable<string>>> Parse(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);


            var div = htmlDocument.DocumentNode.Descendants("div")
                .FirstOrDefault(d => d.HasClass("nav-text"));

            if (div == null)
                throw new Exception("Can not find next button");


            Console.WriteLine(div.InnerText);
        
            throw new NotImplementedException();
        }
    }
}
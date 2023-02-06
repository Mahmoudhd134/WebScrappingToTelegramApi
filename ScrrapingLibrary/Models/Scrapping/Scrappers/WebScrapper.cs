using ScrapingLibrary.Models.WebSites;

namespace ScrapingLibrary.Models.Scrapping.Scrappers
{
    public class WebScrapper<T> : IScrapper<T>
    {
        private readonly WebSite<T> _webSite;

        public WebScrapper(WebSite<T> webSite)
        {
            _webSite = webSite;
        }

        public string GetUrl()
        {
            return _webSite.Url;
        }

        public async Task<T> GetData()
        {
            return await _webSite.Parse(await _webSite.GetHtml());
        }
    }
}
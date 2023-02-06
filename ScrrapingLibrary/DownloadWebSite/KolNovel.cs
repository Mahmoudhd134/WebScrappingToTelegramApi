using System.Web;
using HtmlAgilityPack;

namespace ScrapingLibrary.DownloadWebSite
{
    public class KolNovel
    {
        private readonly string _startupPage;
        private readonly HtmlDocument _htmlDocument;

        public KolNovel(string startupPage)
        {
            _startupPage = startupPage;
            _htmlDocument = new HtmlDocument();
        }

        public async Task<bool> Download(string path)
        {
            var homeDir = path;
            var cssDir = $"\\css";
            var jsDir = $"\\js";

            if (Directory.Exists(homeDir) == false)
                Directory.CreateDirectory(homeDir);
            if (Directory.Exists(path + cssDir) == false)
                Directory.CreateDirectory(path + cssDir);
            if (Directory.Exists(path + jsDir) == false)
                Directory.CreateDirectory(path + jsDir);

            Dictionary<string, string> cssFiles = new();
            Dictionary<string, string> jsFiles = new();

            var pageUrl = _startupPage;
            var i = 0;
            while (true)
            {
                if (string.IsNullOrWhiteSpace(pageUrl))
                    break;
                i++;
                _htmlDocument.LoadHtml(await GetContent(pageUrl));

                if (i == 1)
                {
                    Console.WriteLine($"Downloading Css Files...");
                    await GetCssFiles(cssFiles, homeDir, cssDir);
                    Console.WriteLine($"Download Complete.");
                    Console.WriteLine($"Downloading JS (JavaScript) Files...");
                    await GetJsFiles(jsFiles, homeDir, jsDir);
                    Console.WriteLine($"Download Complete.");
                }

                try
                {
                    UpdateCssPaths(cssFiles);
                    UpdateJsPaths(jsFiles);
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine($"Downloading Css Files...");
                    await GetCssFiles(cssFiles, homeDir, cssDir);
                    Console.WriteLine($"Download Complete.");
                    Console.WriteLine($"Downloading JS (JavaScript) Files...");
                    await GetJsFiles(jsFiles, homeDir, jsDir);
                    Console.WriteLine($"Download Complete.");
                }

                var (nextLink, prevLink) = GetNextAndPrevLinks();

                var pageName = GetFileNameFromLink(pageUrl, i);
                var nextPageName = GetFileNameFromLink(nextLink, i + 1);
                var prevPageName = GetFileNameFromLink(prevLink, i - 1);

                foreach (var htmlNode in GetLinksByRel("next"))
                    htmlNode.SetAttributeValue("href", nextPageName);

                foreach (var htmlNode in GetLinksByRel("prev"))
                    htmlNode.SetAttributeValue("href", prevPageName);


                _htmlDocument.Save($"{homeDir}\\{pageName}");

                Console.WriteLine($"Page ({i}) >> {HttpUtility.UrlDecode(pageUrl)} >> Done");
                pageUrl = nextLink;
            }

            return true;
        }

        private async Task GetCssFiles(Dictionary<string, string> cssFiles, string homePath, string cssPath)
        {
            var linkStyleSheets = this._htmlDocument.DocumentNode
                .Descendants("link")
                .Where(l => l.GetAttributeValue("rel", "f_13@").Equals("stylesheet") &&
                            l.GetAttributeValue("id", "").Length > 0 &&
                            l.GetAttributeValue("href", "").Length > 0);

            foreach (var linkStyleSheet in linkStyleSheets)
            {
                var url = linkStyleSheet.GetAttributeValue("href", "");
                var id = linkStyleSheet.Id;
            
                if(url.StartsWith("http") == false)
                    continue;

                var cssContent = await GetContent(url);

                var fileRelativePath = $"{cssPath}\\{id}.css";
                var path = $"{homePath}\\{fileRelativePath}";
                await File.WriteAllTextAsync(path, cssContent);

                try
                {
                    cssFiles.Add(id, fileRelativePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($">>>>> Error\t {e.Message}");
                }
            }
        }

        private async Task GetJsFiles(Dictionary<string, string> jsFiles, string homePath, string jsPath)
        {
            var linkJavascripts = this._htmlDocument.DocumentNode
                .Descendants("script")
                .Where(l => l.GetAttributeValue("type", "f_13@").Equals("text/javascript") &&
                            l.GetAttributeValue("id", "").Length > 0 &&
                            l.GetAttributeValue("src", "").Length > 0);


            foreach (var linkJavascript in linkJavascripts)
            {
                var url = linkJavascript.GetAttributeValue("src", "");
                var id = linkJavascript.Id;
            
                if(url.StartsWith("http") == false)
                    continue;

                var jsContent = await GetContent(url);

                var fileRelativePath = $"{jsPath}\\{id}.js";
                var path = $"{homePath}\\{fileRelativePath}";
                await File.WriteAllTextAsync(path, jsContent);

                try
                {
                    jsFiles.Add(id, fileRelativePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($">>>>> Error\t {e.Message}");
                }
            }
        }

        private void UpdateCssPaths(Dictionary<string, string> cssFiles)
        {
            var linkStyleSheets = this._htmlDocument.DocumentNode
                .Descendants("link")
                .Where(l => l.GetAttributeValue("rel", "f_13@").Equals("stylesheet") &&
                            l.GetAttributeValue("id", "").Length > 0 &&
                            l.GetAttributeValue("href", "").Length > 0);

            foreach (var linkStyleSheet in linkStyleSheets)
                linkStyleSheet.SetAttributeValue("href", cssFiles[linkStyleSheet.Id]);
        }

        private void UpdateJsPaths(Dictionary<string, string> jsFiles)
        {
            var linkJavascripts = this._htmlDocument.DocumentNode
                .Descendants("script")
                .Where(l => l.GetAttributeValue("type", "f_13@").Equals("text/javascript") &&
                            l.GetAttributeValue("id", "").Length > 0 &&
                            l.GetAttributeValue("src", "").Length > 0);

            foreach (var linkJavascript in linkJavascripts)
                linkJavascript.SetAttributeValue("src", jsFiles[linkJavascript.Id]);
        }

        private string GetFileNameFromLink(string url, int i)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";
            return HttpUtility.UrlDecode(url).Split("/")
                .SkipLast(1)
                .Last() + $"-الفصل{i}-.html";
        }

        private (string?, string?) GetNextAndPrevLinks()
        {
            return (GetLinkByRel("next")?.GetAttributeValue("href", "NO_LINK"),
                GetLinkByRel("prev")?.GetAttributeValue("href", "NO_LINK"));
        }

        private HtmlNode? GetLinkByRel(string rel)
        {
            return _htmlDocument.DocumentNode.Descendants("a")
                .FirstOrDefault(l => l.GetAttributeValue("href", "").Length > 0 &&
                                     l.GetAttributeValue("rel", "fdsjak9").Equals(rel));
        }

        private IEnumerable<HtmlNode> GetLinksByRel(string rel)
        {
            return _htmlDocument.DocumentNode.Descendants("a")
                .Where(l => l.GetAttributeValue("href", "").Length > 0 &&
                            l.GetAttributeValue("rel", "342fsdjka").Equals(rel));
        }

        private async Task<string> GetContent(string url)
        {
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            try
            {
                return await httpClient.GetStringAsync(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
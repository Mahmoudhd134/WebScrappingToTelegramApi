using ConsoleApp.Helpers;
using ScrapingLibrary.Models.Scrapping.Scrappers;
using ScrapingLibrary.Models.WebSites.Sites;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ConsoleApp.Controllers;

public class ScrappingSites
{
    public static async Task MakeDocxFile(string dir)
    {
        var files = Directory.GetFiles(dir).Where(p => p.Split(".").Last().Equals("txt"));

        foreach (var file in files)
        {
            var content = (await File.ReadAllLinesAsync(file)).Select(s => s + "\n");

            var fileName = $"{string.Join(".", file.Split(".").SkipLast(1))}.docx";
            using var doc = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document);


            var mainPart = doc.AddMainDocumentPart();
            var document = new Document();
            var body = new Body();

            var settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings(new Languages() { Val = "ar-SA" });
            settingsPart.Settings.Save();


            foreach (var line in content)
            {
                var para = new Paragraph();
                var run = new Run();
                var text = new Text(line);

                var paraProps = new ParagraphProperties()
                {
                    BiDi = new BiDi(),
                    Justification = new Justification()
                    {
                        Val = JustificationValues.Right
                    }
                };
                para.Append(paraProps);

                var runProperties = new RunProperties()
                {
                    FontSize =
                    {
                        Val = "60"
                    }
                };
                run.Append(runProperties);

                run.Append(text);
                para.Append(run);
                body.Append(para);
            }

            document.Append(body);
            mainPart.Document = document;
            Console.Write($"\rFile {file} Done!!");
        }
    }

    public static async Task MakeWordFileV2(string dir)
    {
        var files = Directory.GetFiles(dir).Where(p => p.Split(".").Last().Equals("txt"));

        // foreach (var file in files)
        // {
        // var fileName = $"{string.Join(".", file.Split(".").SkipLast(1))}.docx";

        // }
    }

    public static async Task MakePdfFile(string dir)
    {
        var files = Directory.GetFiles(dir).Where(p => p.Split(".").Last().Equals("txt"));

        foreach (var file in files)
        {
            var content = (await File.ReadAllLinesAsync(file))
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(s => s + "<br/>");

            var fileName = $"{string.Join(".", file.Split(".").SkipLast(1))}.pdf";
            var renderer = new IronPdf.HtmlToPdf();

            var pdf = renderer.RenderHtmlAsPdf("<html><body dir=\"rtl\" style=\"font-size:3em;\"><p>" +
                                               string.Join("<br/>", content)
                                               + "</p></body></html>");
            pdf.SaveAs(fileName);
            Console.Write($"\rFile {file} Done!!");
        }
    }

    public static async Task ScrapeKolNovel(string url, string dir, int numberOfChaptersPerFile = 200,
        int whiteLinesBetweenLines = 1)
    {
        if (Directory.Exists(dir) == false)
            Directory.CreateDirectory(dir);
        var pagesScrapper = new WebScrapper<IEnumerable<string>>(new KolNovelPages(url));
        var pages = (await pagesScrapper.GetData()).ToList();
        var chapters = new List<IEnumerable<string>>();
        var i = 0;
        for (; i < pages.Count; i++)
        {
            if (i % numberOfChaptersPerFile == 0 && i != 0)
            {
                var d = $@"{dir}\{i + 1 - numberOfChaptersPerFile}-{i}.txt";
                await File.WriteAllLinesAsync(d, chapters
                    .Select(c => string.Join("\n".Repeat(whiteLinesBetweenLines + 1), c)));
                chapters = new List<IEnumerable<string>>();
            }

            var page = pages[i];
            var chapterScrapper = new WebScrapper<IEnumerable<string>>(new KolNovel(page));
            var chapter = (await chapterScrapper.GetData()).ToList().Append("-".Repeat(100));
            chapters.Add(chapter);
            Console.Write($"\rNumber ({i}) Done!!");
        }

        var d2 =
            $@"{dir}\{i + 1 - (i % numberOfChaptersPerFile == 0 ? numberOfChaptersPerFile : i % numberOfChaptersPerFile)}-{i}.txt";
        await File.WriteAllLinesAsync(d2, chapters
            .Select(c => string.Join("\n".Repeat(whiteLinesBetweenLines + 1), c)));
    }
}
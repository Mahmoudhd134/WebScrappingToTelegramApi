using Newtonsoft.Json;
using ScrapingLibrary.Models.Pdf.Pdfs;
using ScrapingLibrary.Models.Quiz;
using ScrapingLibrary.Models.Scrapping;
using ScrapingLibrary.Models.Scrapping.Scrappers;
using ScrapingLibrary.Models.WebSites.Sites;

namespace ConsoleApp.Controllers;

public class AutomateSendPoll
{
    private readonly IQuizModelTelegramSender _sender;
    private readonly string _chatId;

    public AutomateSendPoll(string chatId, IQuizModelTelegramSender sender)
    {
        _chatId = chatId;
        _sender = sender;
    }

    public async Task StartOSPdf(string path)
    {
        IScrapper<IEnumerable<QuizModel>> scrapper = new PdfScrapper<IEnumerable<QuizModel>>(new OSPdfs(path));
        if (await _sender.Send(_chatId, await scrapper.GetData()) == false)
            throw new Exception("Error Happened");
    }

    public async Task StartPdf(string path)
    {
        IScrapper<IEnumerable<QuizModel>> scrapper = new PdfScrapper<IEnumerable<QuizModel>>(new ImageQuizPdf(path));
        if (await _sender.Send(_chatId, await scrapper.GetData()) == false)
            throw new Exception("Error Happened");
    }

    public async Task StartEcamveda(string startingUrl)
    {
        var links = await new WebScrapper<IEnumerable<string>>(
                new ExamvedaPages(startingUrl))
            .GetData();

        var allLinks = new List<string>() { startingUrl };
        allLinks.AddRange(links);

        IEnumerable<WebScrapper<IEnumerable<QuizModel>>> WebScrappers()
        {
            foreach (var site in allLinks)
                yield return new WebScrapper<IEnumerable<QuizModel>>(new Examveda(site));
        }

        foreach (var webScrapper in WebScrappers())
            if (await _sender.Send(_chatId, await webScrapper.GetData()) == false)
                throw new Exception("Error Happen");
    }

    public async Task StartSanFoundryAI()
    {
        var scrapper = new WebScrapper<IEnumerable<string>>(
            new SanFoundryWebSitePages(
                "https://www.sanfoundry.com/1000-neural-networks-questions-answers/"));

        var links = await scrapper.GetData();

        IEnumerable<WebScrapper<IEnumerable<QuizModel>>> WebScrappers()
        {
            foreach (var site in links.Skip(6).Take(23))
                yield return new WebScrapper<IEnumerable<QuizModel>>(new SanFoundryAltimate(site));
        }

        // foreach (var webScrapper in WebScrappers())
        // if (await _sender.Send(_chatId, await webScrapper.GetData()) == false)
        // throw new Exception("Error Happen");

        List<QuizModel> quizModels = new();
        foreach (var webScrapper in WebScrappers())
            quizModels.AddRange(await webScrapper.GetData());
        await MakeAFileAsync(quizModels, @"C:\Users\nasse\OneDrive\Desktop\d.txt");
    }

    public async Task StartSanFoundry(string url, string path = null)
    {
        var scrapper = new WebScrapper<IEnumerable<QuizModel>>(new SanFoundryAltimate(url));
        var quizModels = (await scrapper.GetData()).ToList();
        await Send(quizModels);

        if (string.IsNullOrWhiteSpace(path) == false)
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(quizModels));
    }

    public async Task StartStackHowTo(string url, string path = null)
    {
        var scrapper = new WebScrapper<IEnumerable<QuizModel>>(new Stackhowto(url));
        var quizModels = (await scrapper.GetData()).ToList();
        await Send(quizModels);

        if (string.IsNullOrWhiteSpace(path) == false)
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(quizModels));
    }

    public async Task StartSanFoundryAlgorithms()
    {
        IEnumerable<string> GetSites()
        {
            var sites = new List<string>();
            sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-linear-search-iterative/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-binary-search-iterative/");
            // sites.Add("https://www.sanfoundry.com/insertion-sort-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/insertion-sort-interview-questions-answers/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-selection-sort/");
            // sites.Add("https://www.sanfoundry.com/merge-sort-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/quicksort-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/quicksort-interview-questions-answers/");
            // sites.Add("https://www.sanfoundry.com/quicksort-questions-answers-entrance-exams/");
            // sites.Add("https://www.sanfoundry.com/quicksort-random-sampling-multiple-choice-questions-answers-mcqs/");
            // sites.Add(
            //     "https://www.sanfoundry.com/quicksort-median-three-partitioning-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/heapsort-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/heapsort-interview-questions-answers/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-breadth-first-search/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-uniform-binary-search/");
            // sites.Add("https://www.sanfoundry.com/linear-search-recursive-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/masters-theorem-interview-questions-answers/");
            // sites.Add("https://www.sanfoundry.com/masters-theorem-multiple-choice-questions-answers-mcqs/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-quiz/");
            // sites.Add("https://www.sanfoundry.com/data-structure-questions-answers-search-element-array-recursion-1/");
            return sites;
        }

        IEnumerable<WebScrapper<IEnumerable<QuizModel>>> GetScrappers()
        {
            foreach (var site in GetSites())
                yield return new WebScrapper<IEnumerable<QuizModel>>(new SanFoundryAltimate(site));
        }

        foreach (var webScrapper in GetScrappers())
            if (await _sender.Send(_chatId, await webScrapper.GetData()) == false)
                throw new Exception("Error Happen");

        List<QuizModel> quizModels = new();
        foreach (var webScrapper in GetScrappers())
            quizModels.AddRange(await webScrapper.GetData());
        await File.WriteAllTextAsync(@"E:\WebScrappingToTelegramApi\ConsoleApp\wwwroot\files\algo.json",
            JsonConvert.SerializeObject(quizModels));
    }

    public async Task StartSanFoundryImageProcessing()
    {
        var scrapper = new WebScrapper<IEnumerable<string>>(
            new SanFoundryWebSitePages(
                "https://www.sanfoundry.com/1000-digital-image-processing-questions-answers/"));

        var links = await scrapper.GetData();

        IEnumerable<WebScrapper<IEnumerable<QuizModel>>> WebScrappers()
        {
            foreach (var site in links.Take(29))
                yield return new WebScrapper<IEnumerable<QuizModel>>(new SanFoundryAltimate(site));
        }

        foreach (var webScrapper in WebScrappers())
            if (await _sender.Send(_chatId, await webScrapper.GetData()) == false)
                throw new Exception("Error Happen");
    }

    public async Task StartCompsciedu(string url, int startPage, int endPage, string path = null)
    {
        var all = new List<QuizModel>();
        for (; startPage <= endPage; startPage++)
        {
            var scrapper = new WebScrapper<IEnumerable<QuizModel>>(new Compsciedu(url + startPage));
            var quizModels = (await scrapper.GetData()).ToList();
            await Send(quizModels);
            all.AddRange(quizModels);
        }

        if (string.IsNullOrWhiteSpace(path) == false)
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(all));
    }

    public async Task StartWordByWordMCQPdf(string path)
    {
        var scrapper = new PdfScrapper<IEnumerable<QuizModel>>(new WordByWordMcqPdf(path));
        if (await _sender.Send(_chatId, await scrapper.GetData()) == false)
            throw new Exception("Error Happen");
    }

    public async Task StartWordByWordTAndFPdf(string path)
    {
        var scrapper = new PdfScrapper<IEnumerable<QuizModel>>(new WordByWordTAndFPdf(path));
        if (await _sender.Send(_chatId, await scrapper.GetData()) == false)
            throw new Exception("Error Happened");
    }

    public async Task StartJsonFile(string path)
    {
        var content = await File.ReadAllTextAsync(path);
        var quizzes = JsonConvert.DeserializeObject<IEnumerable<QuizModel>>(content)
            .ToList();
        if (await _sender.Send(_chatId, quizzes) == false)
            throw new Exception("Error Happened");
    }

    public async Task MakeAFileAsync(IEnumerable<QuizModel> quizzes, string outPath)
    {
        var text = string.Join("\n\n-------------------------\n\n", quizzes
            .Select(qu =>
                $"Q) {qu.Question.Trim()}\n\n\n" +
                $"{string.Join("\n", qu.GetOptions())}\n\n" +
                $"Answer: {qu.RightAnswer}\n" +
                $"Explanation: {qu.Explanation}"
            ));

        await using var fileStream = new FileStream(outPath, FileMode.Create);
        await using var streamWriter = new StreamWriter(fileStream);
        await streamWriter.WriteLineAsync(text);
    }
    
    public async Task MakeAnHtmlFileAsync(IEnumerable<QuizModel> quizzes, string outPath)
    {
        var text = string.Join("\n\n-------------------------\n\n", quizzes
            .Select(qu =>
                $"Q) {qu.Question.Trim()}\n\n\n" +
                $"{string.Join("\n", qu.GetOptions())}\n\n" +
                $"Answer: {qu.RightAnswer}\n" +
                $"Explanation: {qu.Explanation}"
            ));

        await using var fileStream = new FileStream(outPath, FileMode.Create);
        await using var streamWriter = new StreamWriter(fileStream);
        await streamWriter.WriteLineAsync(text);
    }

    private async Task Send(IEnumerable<QuizModel> quizModels)
    {
        return;
        if (await _sender.Send(_chatId, quizModels) == false)
            throw new Exception("Error Happen");
    }

    private async Task Send(QuizModel quizModel)
    {
        if (await _sender.Send(_chatId, quizModel) == false)
            throw new Exception("Error Happen");
    }
}
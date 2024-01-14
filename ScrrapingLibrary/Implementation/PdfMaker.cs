using System.Diagnostics;
using ScrapingLibrary.Helpers;
using ScrapingLibrary.Models.Quiz;
using ScrapingLibrary.Services;

namespace ScrapingLibrary.Implementation;

public class PdfMaker : IPdfMaker
{
    public async Task MakePdf(IEnumerable<QuizModel> quizModels, string outputPath)
    {
        var html = MakeHtmlContent(quizModels);
        var tempHtmlPath = Path.Combine(Path.GetDirectoryName(outputPath), Guid.NewGuid() + "temp.html");
        await File.WriteAllTextAsync(tempHtmlPath, html);
        ConvertToPdf(tempHtmlPath, outputPath);
        File.Delete(tempHtmlPath);
    }

    private static string MakeHtmlContent(IEnumerable<QuizModel> quizModels)
    {
        var correctAnswerStyle = "style=\"background-color:yellow;width:fit-content;\"";
        var refactored = quizModels.Select(q =>
            $"Q) {q.Question.Trim()}{"<br/>".Repeat(3)}" +
            $"{string.Join("<br/>", q.GetOptions().Select((a, i) => $"<div {(q.GetCorrectOptionId() == i ? correctAnswerStyle : "")}>{a}</div>"))}{"<br/>".Repeat(2)}" +
            $"Answer: {q.RightAnswer}<br/>" +
            (string.IsNullOrWhiteSpace(q.Explanation)
                ? null
                : $"<br/><br/>Explanation: {q.Explanation}")
        ).ToList();

        var random = new Random();
        int R() => random.Next(1, 4);
        return "<!DOCTYPE html>" +
               "<html><head>" +
               "<meta charset=\"UTF-8\">" +
               "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
               "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
               "<title>Document</title>" +
               $"</head><body style=\"font-size:1rem;word-spacing:.125rem;\">" +
               string.Join("<hr/>",
                   refactored.Select((x, i) => $"<div style=\"padding:10px;border:3px solid black;\">" +
                                               $"<div style=\"margin:10px 0px;border-bottom:1px solid black;width:fit-content;\">Question {i + 1} of {refactored.Count}</div>" +
                                               (i % 10 == 0
                                                   ? $"<div style=\"display:flex;\">" +
                                                     $"<img style=\"height:70px;margin:10px;margin-right:auto;object-fit: contain;\" src=\"{R()}.jpg\"/>" +
                                                     $"</div>"
                                                   : "") +
                                               $"{x}" +
                                               $"</div>")) +
               "</body></html>";
    }

    private static void ConvertToPdf(string htmlPath, string outputPdfPath)
    {
        if (Directory.Exists(Directory.GetDirectoryRoot(outputPdfPath)) == false)
            Directory.CreateDirectory(outputPdfPath);

        for (var i = 0; i < 3; i++)
        {
            var process = new Process();
            process.StartInfo.FileName = "wkhtmltopdf.exe";
            process.StartInfo.Arguments = $"--enable-local-file-access \"{htmlPath}\" \"{outputPdfPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            Console.WriteLine("\nStart Converting...");

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
                break;

            Console.WriteLine(
                $"\n\nConversion failed with exit code {process.ExitCode}\nfrom <<{htmlPath}>> to <<{outputPdfPath}>>");
            Console.WriteLine($"trying number {i + 1} failed");
            if (i == 2)
                throw new CanNotMakePdfWithWkHtmlToPdfException(
                    $"\nConversion failed with exit code {process.ExitCode}\nfrom <<{htmlPath}>> to <<{outputPdfPath}>>");

            Console.WriteLine("Retrying...");
            Console.WriteLine();
        }
    }
}
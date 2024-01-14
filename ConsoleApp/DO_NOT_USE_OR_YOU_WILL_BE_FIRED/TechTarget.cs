using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ConsoleApp.DO_NOT_USE_OR_YOU_WILL_BE_FIRED;

public class TechTarget
{
    private readonly string _htmlFilePath =
        @"E:\WebScrappingToTelegramApi\ConsoleApp\DO_NOT_USE_OR_YOU_WILL_BE_FIRED\techtarget.html";

    public async Task<IList<QuizModel>> Parse()
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(await File.ReadAllTextAsync(_htmlFilePath));

        var questions = doc.DocumentNode.Descendants("div")
            .Where(d => d.GetAttributeValue("class", "") == "quizQuestionContainer")
            .Select(q =>
            {
                var quiz = new QuizModel();
                var questionNumber = q.ChildNodes.First(x => x.Name == "h3").InnerText;
                var questionText = q.Descendants().First(d => d.Name == "strong").InnerText;
                quiz.Question = TAD($"{questionNumber}\n{questionText}");
                string explanation = null;
                var options = q.Descendants("ul")
                    .First()
                    .ChildNodes
                    .Where(x => x.Name == "li")
                    .Select((o, i) =>
                    {
                        if (o.GetAttributeValue("class", "") == "correct")
                        {
                            quiz.RightAnswer = QuizModel.GetCorrectOptionChar(i).ToString();
                            quiz.Explanation = string.Join("\n", o.ChildNodes
                                .Where(x => x.Name == "p")
                                .Select(x => TAD(x.InnerText)));
                        }

                        var option = o.ChildNodes.First(x => x.Name == "span").InnerText;
                        return TAD(option);
                    })
                    .ToList();

                if (options.Count > 0)
                    quiz.AnswerA = options[0];
                if (options.Count > 1)
                    quiz.AnswerB = options[1];
                if (options.Count > 2)
                    quiz.AnswerC = options[2];
                if (options.Count > 3)
                    quiz.AnswerD = options[3];
                if (options.Count > 4)
                    quiz.AnswerE = options[4];

                return quiz;
            })
            .ToList();

        return questions;
    }

    private string TAD(string s) => HttpUtility.HtmlDecode(s.Trim());
}
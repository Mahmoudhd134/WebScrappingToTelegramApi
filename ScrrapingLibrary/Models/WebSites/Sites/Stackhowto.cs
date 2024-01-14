using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites;

public class Stackhowto : WebSite<IEnumerable<QuizModel>>
{
    public Stackhowto(string url) : base(url)
    {
    }

    public override Task<IEnumerable<QuizModel>> Parse(string html) =>
        Task.FromResult(QuizModels(html));

    private IEnumerable<QuizModel> QuizModels(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var q = new List<QuizModel>();
        var strings = htmlDocument.DocumentNode
            .Descendants("div")
            .First(d => d.GetAttributeValue("class", "").Equals("cm-entry-summary"))
            .InnerText;

        var wholeText = strings
            .Split("\n")
            .Select(s => HttpUtility.HtmlDecode(s.Trim()))
            .Where(s => string.IsNullOrWhiteSpace(s) == false &&
                        s.ToLower().Equals("discuss") == false &&
                        s.ToLower().Equals("report") == false &&
                        s.ToLower().Equals("too difficult!") == false &&
                        s.ToLower().Equals("view solution") == false)
            .Skip(1)
            .SkipLast(2)
            .ToList();

        var mode = Struct.Nothing;
        var currentQuizNum = 1;
        var quiz = new QuizModel();
        for (var i = 0; i < wholeText.Count; i++)
        {
            var line = wholeText[i];
            if (line.Trim().StartsWith($"{currentQuizNum}."))
            {
                if (mode == Struct.Explanation) //the previuos question ends
                {
                    Console.Write($"\rQuiz Number {currentQuizNum} has been Obtained");
                    q.Add(quiz);
                    quiz = new QuizModel();
                }

                currentQuizNum++;
                mode = Struct.Question;
                quiz.Question = line;
            }
            else if (line.Trim().StartsWith("A "))
            {
                mode = Struct.AnswerA;
                quiz.AnswerA = line;
            }
            else if (line.Trim().StartsWith("B "))
            {
                mode = Struct.AnswerB;
                quiz.AnswerB = line;
            }
            else if (line.Trim().StartsWith("C "))
            {
                mode = Struct.AnswerC;
                quiz.AnswerC = line;
            }
            else if (line.Trim().StartsWith("D "))
            {
                mode = Struct.AnswerD;
                quiz.AnswerD = line;
            }
            else if (line.Trim().StartsWith("E "))
            {
                mode = Struct.AnswerE;
                quiz.AnswerE = line;
            }
            else if (line.Trim() == "Answer")
            {
                mode = Struct.Explanation;
                var correctAnswer = wholeText[++i].Trim();
                quiz.RightAnswer = correctAnswer;
                if (i < wholeText.Count - 1 && wholeText[i + 1].StartsWith(currentQuizNum + ".") == false)
                    quiz.Explanation = wholeText[++i];
            }
            else if (line.Trim().ToLower().StartsWith("explanation:"))
            {
                mode = Struct.Explanation;
                quiz.Explanation = line;
            }
            else

            {
                switch (mode)
                {
                    case Struct.Question:
                        quiz.Question += "\n" + line;
                        break;

                    case Struct.AnswerA:
                        quiz.AnswerA += "\n" + line;
                        break;

                    case Struct.AnswerB:
                        quiz.AnswerB += "\n" + line;
                        break;

                    case Struct.AnswerC:
                        quiz.AnswerC += "\n" + line;
                        break;

                    case Struct.AnswerD:
                        quiz.AnswerD += "\n" + line;
                        break;

                    case Struct.AnswerE:
                        quiz.AnswerE += "\n" + line;
                        break;

                    case Struct.Explanation:
                        quiz.Explanation += "\n" + line;
                        break;

                    default:
                        // throw new NotImplementedException("The line starts with un expected value");
                        break;
                }
            }
        }

        if (mode == Struct.Explanation) //the previuos question ends
        {
            Console.Write($"\rQuiz Number {currentQuizNum} has been Obtained");
            q.Add(quiz);
        }

        return q;
    }
}
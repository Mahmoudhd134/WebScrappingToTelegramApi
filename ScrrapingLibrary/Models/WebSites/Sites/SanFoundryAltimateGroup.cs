using System.Text;
using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class SanFoundryAltimateGroup : WebSite<QuizGroup>
    {
        public SanFoundryAltimateGroup(string url) : base(url)
        {
        }

        public override Task<QuizGroup> Parse(string html)
        {
            Console.WriteLine("Parsing");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var quizGroup = GetQuizzes(htmlDocument);
            return Task.FromResult(quizGroup);
        }

        private QuizGroup GetQuizzes(HtmlDocument htmlDocument)
        {
            var title = HttpUtility.HtmlDecode(
                htmlDocument.DocumentNode.Descendants("h1")
                    .FirstOrDefault(n => n.GetAttributeValue("class", "Bla") == "entry-title")?
                    .InnerText);

            return new QuizGroup
            {
                Title = title,
                QuizModels = QuizModels(htmlDocument)
            };
        }

        private IEnumerable<QuizModel> QuizModels(HtmlDocument htmlDocument)
        {
            var text = GetText(htmlDocument.DocumentNode.SelectSingleNode("//div[@class='" + "entry-content" + "']"));

            var mode = Struct.Nothing;
            var currentQuizNum = 0;
            var quiz = new QuizModel();
            foreach (var l in text.Split("\n").Where(s => string.IsNullOrWhiteSpace(s) == false))
            {
                var line = l;
                if (line.ToLower().StartsWith("sanfoundry global"))
                    break;

                if (line.ToLower().StartsWith("http"))
                    line = $"\n{line}\n";

                if (line.Trim().ToLower().StartsWith((currentQuizNum + 1).ToString()))
                {
                    if (mode == Struct.Explanation) //the previuos question ends
                    {
                        Console.Write($"\rQuiz Number {currentQuizNum} has been Obtained");
                        yield return quiz;
                        quiz = new QuizModel();
                    }

                    currentQuizNum++;
                    mode = Struct.Question;
                    quiz.Question = line;
                }
                else if (line.Trim().ToLower().StartsWith("a)"))
                {
                    mode = Struct.AnswerA;
                    quiz.AnswerA = line;
                }
                else if (line.Trim().ToLower().StartsWith("b)"))
                {
                    mode = Struct.AnswerB;
                    quiz.AnswerB = line;
                }
                else if (line.Trim().ToLower().StartsWith("c)"))
                {
                    mode = Struct.AnswerC;
                    quiz.AnswerC = line;
                }
                else if (line.Trim().ToLower().StartsWith("d)"))
                {
                    mode = Struct.AnswerD;
                    quiz.AnswerD = line;
                }
                else if (line.Trim().ToLower().StartsWith("answer:"))
                {
                    mode = Struct.RightAnswer;
                    var correctAnswer = line.Split(" ").Last();
                    quiz.RightAnswer = correctAnswer.First().ToString();
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
                yield return quiz;
            }
        }

        private string GetText(HtmlNode node)
        {
            if ((node.GetAttributeValue("style", "Bla").Equals("margin:30px 0px;") == false &&
                 (node.GetAttributeValue("class", "Bla").Equals("sf-mobile-ads") ||
                  node.GetAttributeValue("class", "Bla").Equals("sf-desktop-ads")) == false) == false)
                return "";

            if (node.InnerText.Trim().ToLower().Equals("view answer"))
                return "";

            if (node.Name.Equals("img"))
            {
                var url = node.GetAttributeValue("src", "LOAD IMG ERROR");
                return "\n" +
                       (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://") ? url : "") +
                       "\n";
            }
            else if (node.HasChildNodes == false)
                return HttpUtility.HtmlDecode(node.InnerText);

            // not an image and not a text
            var sb = new StringBuilder();
            foreach (var child in node.ChildNodes)
                sb.Append(GetText(child));
            return sb.ToString();
        }
    }
}
using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class Compsciedu : WebSite<IEnumerable<QuizModel>>
    {
        public Compsciedu(string url) : base(url)
        {
        }

        public async override Task<IEnumerable<QuizModel>> Parse(string html)
        {
            Console.WriteLine("\nParsing");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var firstQuesNoAsString = string.Join("",
                htmlDocument.DocumentNode.Descendants("div")
                    .First(d => d.GetAttributeValue("class", "").Equals("innerDiv"))
                    .ChildNodes
                    .First(c => c.GetAttributeValue("class", "").Equals("quescontainer"))
                    .InnerText
                    .Split("\n")
                    .First(s => string.IsNullOrWhiteSpace(s) == false)
                    .Trim()
                    .SkipLast(1));

            var no = int.Parse(firstQuesNoAsString);
            var quizzes = QuizModels(htmlDocument, ref no);
            return quizzes;
        }

        private IList<QuizModel> QuizModels(HtmlDocument htmlDocument, ref int currentQuizNum)
        {
            var q = new List<QuizModel>();
            var strings = htmlDocument.DocumentNode
                .Descendants("div")
                .First(d => d.GetAttributeValue("class", "").Equals("innerDiv"))
                .InnerText;

            var wholeText = string
                .Join("\n", strings)
                .Split("\n")
                .Select(s => HttpUtility.HtmlDecode(s.Trim()))
                .Where(s => s.Length > 0 && s.ToLower().Equals("discuss") == false &&
                            s.ToLower().Equals("report") == false &&
                            s.ToLower().Equals("too difficult!") == false &&
                            s.ToLower().Equals("view answer") == false &&
                            string.IsNullOrWhiteSpace(s) == false);

            var mode = Struct.Nothing;
            var quiz = new QuizModel();
            foreach (var line in wholeText)
            {
                if (line.ToLower().StartsWith("sanfoundry global"))
                    break;

                // if (line.ToLower().StartsWith("http"))
                // {
                //     quiz.Img = line;
                //     continue;
                // }

                if (line.Trim().ToLower().StartsWith((currentQuizNum).ToString()))
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
                else if (line.Trim().ToLower().StartsWith("a."))
                {
                    mode = Struct.AnswerA;
                    quiz.AnswerA = line;
                }
                else if (line.Trim().ToLower().StartsWith("b."))
                {
                    mode = Struct.AnswerB;
                    quiz.AnswerB = line;
                }
                else if (line.Trim().ToLower().StartsWith("c."))
                {
                    mode = Struct.AnswerC;
                    quiz.AnswerC = line;
                }
                else if (line.Trim().ToLower().StartsWith("d."))
                {
                    mode = Struct.AnswerD;
                    quiz.AnswerD = line;
                }
                else if (line.Trim().ToLower().StartsWith("answer:"))
                {
                    mode = Struct.Explanation;
                    var correctAnswer = line[9];
                    quiz.RightAnswer = char.ToString(correctAnswer);
                    quiz.Explanation = "NO EXPLENATION";
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
                            quiz.Explanation  += "\n" + line;
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

        public virtual async Task<string> GetHtml(string url)
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
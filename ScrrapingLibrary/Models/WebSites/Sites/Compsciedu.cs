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
                htmlDocument.DocumentNode.Descendants("span")
                    .First(s => s.GetAttributeValue("class", "") == "fw-bold mx-3")
                    .InnerText
                    .Trim()
                    .Skip(1)
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
                .First(d => d.GetAttributeValue("class", "").Equals("col-lg-6"))
                .InnerText;

            var wholeText = strings
                .Split("\n")
                .Select(s => HttpUtility.HtmlDecode(s.Trim()))
                .Where(s => string.IsNullOrWhiteSpace(s) == false &&
                            s.ToLower().Equals("discuss") == false &&
                            s.ToLower().Equals("report") == false &&
                            s.ToLower().Equals("too difficult!") == false &&
                            s.ToLower().Equals("view solution") == false);

            var mode = Struct.Nothing;
            var quiz = new QuizModel();
            foreach (var line in wholeText)
            {
                if (line.Trim().StartsWith($"Q{currentQuizNum}."))
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
                    quiz.Explanation = null;
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
                q.Add(quiz);
            }

            string ReplaceFirstNewLineWithSpace(string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return s;

                var splits = s.Split("\n");
                return (splits[0] + " " + string.Join("\n", splits.Skip(1))).Trim();
            }

            return q.Select(x => new QuizModel()
                {
                    Question = ReplaceFirstNewLineWithSpace(x.Question),
                    AnswerA = ReplaceFirstNewLineWithSpace(x.AnswerA),
                    AnswerB = ReplaceFirstNewLineWithSpace(x.AnswerB),
                    AnswerC = ReplaceFirstNewLineWithSpace(x.AnswerC),
                    AnswerD = ReplaceFirstNewLineWithSpace(x.AnswerD),
                    AnswerE = ReplaceFirstNewLineWithSpace(x.AnswerE),
                    RightAnswer = ReplaceFirstNewLineWithSpace(x.RightAnswer),
                    Explanation = ReplaceFirstNewLineWithSpace(x.Explanation)
                })
                .ToList();
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
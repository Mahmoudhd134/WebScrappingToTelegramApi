using System.Text;
using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class JavaPoint : WebSite<IEnumerable<QuizModel>>
    {
        public JavaPoint(string url) : base(url)
        {
        }

        public async override Task<IEnumerable<QuizModel>> Parse(string html)
        {
            Console.WriteLine("Parsing");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var quizzes = QuizModels(htmlDocument);

            return quizzes;
        }

        private IEnumerable<QuizModel> QuizModels(HtmlDocument htmlDocument)
        {
            var div = htmlDocument
                .GetElementbyId("city")
                .InnerText
                .Split("\n")
                .Where(s => string.IsNullOrWhiteSpace(s) == false &&
                            s.ToLower().Equals("show answer") == false &&
                            s.ToLower().Equals("workspace") == false)
                .ToList();

            var quiz = new QuizModel();
            bool last = false;
            bool isQ = false;
            int qN = 1;
            for (var i = 0; i < div.Count; i++)
            {
                if (div[i].ToLower().StartsWith((qN).ToString() + ")"))
                {
                    qN++;
                    if (last)
                    {
                        yield return quiz;
                        quiz = new QuizModel();
                    }

                    quiz.Question = div[i];
                    isQ = true;
                }
                else if (div[i].ToLower().StartsWith("answer:"))
                {
                    var c = div[i].Split(" ")[2];

                    quiz.RightAnswer =
                        c.Equals(quiz.AnswerA) ? "a" :
                        c.Equals(quiz.AnswerB) ? "b" :
                        c.Equals(quiz.AnswerC) ? "c" :
                        "d";
                }
                else if (div[i].ToLower().StartsWith("explanation:"))
                {
                    quiz.Explanation = div[i];
                    last = true;
                }
                else
                {
                    if (isQ == false)
                        continue;
                    isQ = false;
                    quiz.AnswerA = div[i];
                    quiz.AnswerB = div[i + 1];
                    i += 1;
                    if (quiz.AnswerA.ToLower().Equals("true") == false)
                    {
                        quiz.AnswerC = div[i + 1];
                        quiz.AnswerD = div[i + 2];
                        i += 2;
                    }
                }
            }
        }

        private string GetText(HtmlNode node)
        {
            if (node.InnerText.Trim().ToLower().Equals("show answer") ||
                node.InnerText.Trim().ToLower().Equals("workspace"))
                return "";


            if (node.Name.Equals("img"))
            {
                var url = node.GetAttributeValue("src", "LOAD IMG ERROR");
                return "\n" +
                       (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://") ? url : "") +
                       "\n";
            }
            else if (node.HasChildNodes == false)
            {
                return HttpUtility.HtmlDecode(node.InnerText);
            }

            // not an image and not a text
            var sb = new StringBuilder();
            foreach (var child in node.ChildNodes)
                sb.Append(GetText(child));
            return sb.ToString();
        }
    }
}
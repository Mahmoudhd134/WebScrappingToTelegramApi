using System.Web;
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class Examveda : WebSite<IEnumerable<QuizModel>>
    {
        public Examveda(string url) : base(url)
        {
        }

        public override Task<IEnumerable<QuizModel>> Parse(string html)
        {
            return Task.FromResult<IEnumerable<QuizModel>>(Extract(html));
        }

        private IList<QuizModel> Extract(string html)
        {
            var quizzes = new List<QuizModel>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var articles = htmlDoc.DocumentNode.Descendants("article");
            foreach (var article in articles)
            {
                var quiz = new QuizModel();
                if (article.GetAttributeValue("class", "").Equals("question single-question question-type-normal") == false)
                    continue;

                if (article.ChildNodes.Count < 5)
                    continue;

                string question;
                try
                {
                    question = article.ChildNodes.First(n => n.Name.Equals("h2"))
                        .InnerText
                        .Trim();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                quiz.Question = question;

                var answers = article.ChildNodes
                    .First(n => n.Name.Equals("div") &&
                                n.GetAttributeValue("class", "N").Equals("question-inner"))
                    .InnerText
                    .Trim()
                    .Split("\n\n")
                    .Select(a => a.Trim())
                    .Select(HttpUtility.HtmlDecode)
                    .ToList();

                try
                {

                    if (answers.Count == 8)
                    {
                        quiz.AnswerA = $"{answers[0]} {answers[1]}";
                        quiz.AnswerB = $"{answers[2]} {answers[3]}";
                        quiz.AnswerC = $"{answers[4]} {answers[5]}";
                        quiz.AnswerD = $"{answers[6]} {answers[7]}";
                    }
                    else if (answers.Count == 4)
                    {
                        quiz.AnswerA = $"{answers[0]} {answers[1]}";
                        quiz.AnswerB = $"{answers[2]} {answers[3]}";
                        quiz.AnswerC = "---";
                        quiz.AnswerD = "---";
                    }
                    else throw new Exception("Answer Lenght Is Unsupported");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                var answer = article.Descendants().First(n => n.HasAttributes == false &&
                                                              n.Name.Equals("div"))
                    .InnerText
                    .Trim()
                    .Split(" ")
                    .Last()
                    .First()
                    .ToString();

                quiz.RightAnswer = answer;
                quiz.Explanation = "";

                Console.WriteLine($"Quiz has been obtained");
                // yield return quiz;
                quizzes.Add(quiz);
            }

            return quizzes;
        }
    }
}
using HtmlAgilityPack;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.WebSites.Sites
{
    public class ExamRadar : WebSite<IEnumerable<QuizModel>>
    {
        public ExamRadar(string url) : base(url)
        {
        }

        public override Task<IEnumerable<QuizModel>> Parse(string html)
        {
            return Task.FromResult(Execute(html));
        }

        private IEnumerable<QuizModel> Execute(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var questionContainers = htmlDoc.DocumentNode.Descendants("div")
                .Where(d => d.GetAttributeValue("class", "").Equals("kensFaq_listItem"));

            foreach (var questionContainer in questionContainers)
            {
                var quiz = new QuizModel();
                var question = questionContainer.ChildNodes
                    .FindFirst("b")
                    .InnerText;

                quiz.Question = question;

                var answers = questionContainer.Descendants("li")
                    .Select(a => a.InnerText)
                    .ToList();

                quiz.AnswerA = "A. " + answers[0];
                quiz.AnswerB = "B. " + answers[1];
                quiz.AnswerC = "C. " + answers[2];
                quiz.AnswerD = "D. " + answers[3];

                var rightAnswer = questionContainer.Descendants("div")
                    .First(d => d.GetAttributeValue("class", "").Equals("display_answer"))
                    .InnerText
                    .Trim()
                    .First()
                    .ToString();

                quiz.RightAnswer = rightAnswer;
                quiz.Explanation = "";

                yield return quiz;
            }
        }
    }
}
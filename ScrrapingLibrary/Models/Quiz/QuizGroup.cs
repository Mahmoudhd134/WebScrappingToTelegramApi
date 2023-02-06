namespace ScrapingLibrary.Models.Quiz
{
    public class QuizGroup
    {
        public string Title { get; set; }
        public IEnumerable<QuizModel> QuizModels { get; set; }
    }
}
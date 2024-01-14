using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Services;

public interface IPdfMaker
{
    Task MakePdf(IEnumerable<QuizModel> quizModels, string outputPath);
}
namespace ScrapingLibrary.Models.Quiz;

public interface IQuizModelTelegramSender
{
    Task<bool> Send(string chatId, QuizModel quizModel);
    Task<bool> Send(string chatId, IEnumerable<QuizModel> quizModels);
    Task<bool> Send(string chatId, QuizGroup quizGroup);
}
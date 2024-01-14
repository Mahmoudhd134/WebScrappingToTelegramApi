namespace ScrapingLibrary.Models.Quiz;

public interface IQuizModelTelegramSender
{
    Task<bool> Send(string chatId, QuizModel quizModel, int messageReplyId = default);
    Task<bool> Send(string chatId, IEnumerable<QuizModel> quizModels, int messageReplyId = default);
    Task<bool> Send(string chatId, QuizGroup quizGroup, int messageReplyId = default);
}
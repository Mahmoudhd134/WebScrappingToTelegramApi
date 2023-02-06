using ScrapingLibrary.Models.Telegram.Types;

namespace ScrapingLibrary.Models.Telegram
{
    public interface ITelegramBot
    {
        Task<bool> SendMsg(TelegramMsg telegramMsg);
        Task<bool> SendMsg(IEnumerable<TelegramMsg> telegramMessages);
        Task<bool> SendPhoto(TelegramPhoto telegramPhoto);
        Task<bool> SendPhoto(IEnumerable<TelegramPhoto> telegramPhotos);
        Task<bool> SendQuiz(TelegramQuiz telegramQuiz);
        Task<bool> SendQuiz(IEnumerable<TelegramQuiz> telegramQuizzes);
        Task<bool> SendVote(TelegramVote telegramVote);
        Task<bool> SendVote(IEnumerable<TelegramVote> telegramVotes);
    }
}
namespace ScrapingLibrary.Models.Telegram.Types
{
    public class TelegramQuiz : TelegramVote
    {
        public string type { get; } = "quiz";
        public int correct_option_id { get; set; }
    }
}
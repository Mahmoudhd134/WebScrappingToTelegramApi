namespace ScrapingLibrary.Models.Telegram.Types
{
    public class TelegramVote
    {
        public string chat_id { get; set; }
        public string question { get; set; }
        public IEnumerable<string> options { get; set; }
        public bool is_anonymous { get; set; }
        public bool allows_multiple_answers { get; set; } = true;
        public int message_thread_id { get; set; }
    }
}
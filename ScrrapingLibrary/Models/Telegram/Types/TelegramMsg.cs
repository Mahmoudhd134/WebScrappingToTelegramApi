namespace ScrapingLibrary.Models.Telegram.Types
{
    public class TelegramMsg
    {
        public string chat_id { get; set; }
        public int? message_thread_id { get; set; }
        public string text { get; set; }
    }
}
﻿namespace ScrapingLibrary.Models.Telegram.Types
{
    public class TelegramPhoto
    {
        public string chat_id { get; set; }
        public int message_thread_id { get; set; }
        public string photo { get; set; }
    }
}
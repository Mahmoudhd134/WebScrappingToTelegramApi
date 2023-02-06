# ScrapingToTelegram
An scrapper tool to scrap a websites/pdfs for any thing also for sending it to telegram

Written in dotnet 6

You need first to add appsettings.json file in the ConsoleApp dicrectory with the following structure:
```json
{
  "token":"<Your-Telegram-Token-Here>",
  "baseApi:"https://api.telegram.org/bot",
  "chats":{
    "<groupt/chat-name>":"<chat-id>"
  }
}
```

using ConsoleApp.Controllers;
using ConsoleApp.Helpers;
using Microsoft.Extensions.Configuration;
using ScrapingLibrary.Implementation.Telegram;

var telegramConfig = ProgramConfigurations.GetConfigurationRoot("appsettings.json")
    .GetSection("telegram")
    .Get<TelegramInfo>();

//if you but your telegram bot token is appsetting.json file
// var api = telegramConfig.BaseApi + telegramConfig.Token;

//if you want to bass the token as an args
var api = telegramConfig.BaseApi + args[0];

var sender = new TelegramBot(api);

var myChat = telegramConfig.Chats.Ggghhhggg;
var autoSender = new AutomateSendPoll(myChat, sender);

// var dir = @"C:\Users\nasse\OneDrive\Desktop\KolNovel\RI";
// await ScrappingSites.ScrapeKolNovel(
//      "https://kolnovel.com/series/%d8%a7%d9%84%d9%82%d8%b3-%d8%a7%d9%84%d9%85%d8%ac%d9%86%d9%88%d9%86/", dir,numberOfChaptersPerFile:300);



Console.WriteLine();
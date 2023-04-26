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

var dir = @"C:\Users\nasse\OneDrive\Desktop\KolNovel\TEST";
await ScrappingSites.ScrapeKolNovel(
      "https://kolnovel.com/series/%d8%a7%d9%84%d8%b9%d8%b5%d8%b1-%d8%a7%d9%84%d9%85%d9%84%d8%ad%d9%85%d9%8a/", dir,numberOfChaptersPerFile:100);



Console.WriteLine();
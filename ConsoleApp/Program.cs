using ScrapingLibrary.Models.Quiz;
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

IQuizModelTelegramSender sender = new TelegramBot(api);
var autoSender = new AutomateSendPoll(telegramConfig.Chats.DistributedGroup, sender);

// await autoSender.StartSanFoundryImageProcessing();
await autoSender.StartSanFoundryAI();
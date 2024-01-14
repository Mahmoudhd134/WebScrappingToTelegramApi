using System.Text.Json;
using ConsoleApp.Controllers;
using ConsoleApp.DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
using ConsoleApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrapingLibrary.Helpers;
using ScrapingLibrary.Implementation;
using ScrapingLibrary.Implementation.Telegram;
using ScrapingLibrary.Models.Pdf.Pdfs;
using ScrapingLibrary.Models.Quiz;
using ScrapingLibrary.Models.Scrapping.Scrappers;
using ScrapingLibrary.Models.Telegram;
using ScrapingLibrary.Models.Telegram.Types;
using ScrapingLibrary.Services;


var telegramConfig = ProgramConfigurations
    .GetConfigurationRoot("appsettings.json")
    .GetSection("telegram")
    .Get<TelegramInfo>();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IPdfMaker, PdfMaker>()
    .BuildServiceProvider();

//if you but your telegram bot token is appsetting.json file
// var api = telegramConfig.BaseApi + telegramConfig.Token;

//if you want to bass the token as an args
var api = telegramConfig.BaseApi + args[0];

var sender = new TelegramBot(api);

//var myChat = telegramConfig.Chats.Q4kRevisionGroup.Id;
//var myTopic = telegramConfig.Chats.Q4kRevisionGroup.Topics.First(t => t.Name == "Network Management").MessageId;

var myChat = telegramConfig.Chats.Nmv3;
int myTopic = 0;


var myMessageSender = MessageSender(sender, myChat, myTopic);
var myMessagesSender = MessagesSender(sender, myChat, myTopic);

var autoSender = new AutomateSendPoll(myChat, sender);

const string path = @"C:\Users\Mahmoud\OneDrive\Desktop\New folder (2)";

// var lec1 = JsonSerializer.Deserialize<List<QuizModel>>(await File.ReadAllTextAsync(Path.Combine(path, "lec 1.json")));
// var lec2 = JsonSerializer.Deserialize<List<QuizModel>>(await File.ReadAllTextAsync(Path.Combine(path, "lec 2.json")));
// var lec3 = JsonSerializer.Deserialize<List<QuizModel>>(await File.ReadAllTextAsync(Path.Combine(path, "lec 3.json")));
// var lec4 = JsonSerializer.Deserialize<List<QuizModel>>(await File.ReadAllTextAsync(Path.Combine(path, "lec 4.json")));
//
// await myMessageSender("chapter 1 #c");
// if (await sender.Send(myChat, lec1) == false)
//     throw new Exception("bla bla bla");
//
// await myMessageSender("chapter 2 #c");
// if (await sender.Send(myChat, lec2) == false)
//     throw new Exception("bla bla bla");
//
// await myMessageSender("chapter 3 #c");
// if (await sender.Send(myChat, lec3) == false)
//     throw new Exception("bla bla bla");
//
// await myMessageSender("chapter 4 #c");
// if (await sender.Send(myChat, lec4) == false)
//      throw new Exception("bla bla bla");


var scrapper = new PdfScrapper<List<QuizModel>>(new NM(Path.Combine(path, "cd.txt"),
    "1. B, 2. C, 3. B, 4. B, 5. D, 6. D, 7. B, 8. A, 9. D, 10. A, 11. B, 12. A, 13. C, 14. C, 15. A, 16. B, 17. B, 18. A, 19. \nC, 20. C, 21. B, 22. B, 23. C, 24. C, 25. B, 26. B, 27. A, 28. D, 29. A, 30. B, 31. A, 32. C, 33. C, 34. A, 35. B, 36. B, 37. A, 38. C, 39. C, 40. B, 41. B, 42. C, 43. C, 44. B, 45. D, 46. D, 47. D, 48. A, 49. D, 50. B"));
var cdData = await scrapper.GetData();

scrapper = new PdfScrapper<List<QuizModel>>(new NM(Path.Combine(path, "iws.txt"),
    "1. A, 2. C, 3. B, 4. B, 5. A, 6. C, 7. D, 8. B, 9. B, 10. D, 11. B, 12. C, 13. C, 14. A, 15. C, 16. A, 17. B, 18. B, 19. A, 20. A, 21. B, 22. C, 23. B, 24. C, 25. D, 26. B, 27. B, 28. B, 29. A, 30. B, 31. A, 32. B, 33. D, 34. D, 35. A, 36. B, 37. D, 38. C, 39. B, 40. B, 41. A, 42. B, 43. B, 44. B, 45. A,46. B,47. B,48. B,49. B,50. A"));
var iwsData = await scrapper.GetData();

scrapper = new PdfScrapper<List<QuizModel>>(new NmMcq(Path.Combine(path, "nm-mcq.txt")));
var nmMcq = await scrapper.GetData();

scrapper = new PdfScrapper<List<QuizModel>>(new NmTaf(Path.Combine(path, "nm-taf.txt")));
var nmTaf = await scrapper.GetData();

var all = new List<QuizModel>();
all.AddRange(cdData);
all.AddRange(iwsData);
all.AddRange(nmMcq);
all.AddRange(nmTaf);

all.Shuffle();
all.Shuffle();
all.Shuffle();
all.Shuffle();
all.ForEach(x => x.Shuffle());
Console.WriteLine();
await sender.Send(myChat, all, myTopic);
// Console.WriteLine();
// var pdfMaker = new PdfMaker();
// await pdfMaker.MakePdf(all,
//    @"C:\Users\Mahmoud\RiderProjects\WebScrappingToTelegramApi\ConsoleApp\wwwroot\files\network managment.pdf");

//  await myMessagesSender(new List<string>()
//  {
//      "اختبار اختبار test test هل هو يعمل ؟ هل هو يعمل ؟ الله اعلم",
//  });
//
//  await myMessagesSender(new List<string>()
//  {
//      "#اسئلة",
//      "الملف ال اسمه Network Management MCQ",
//      "ال MCQ"
//  });
//  if (await sender.Send(myChat, nmMcq, myTopic) == false)
//      throw new Exception("Something happened!!!");
//
//
//  await myMessagesSender(new List<string>()
//  {
//      "#اسئلة",
//      "الملف ال اسمه Network Management MCQ",
//      "ال صح وغلط"
//  });
//  if (await sender.Send(myChat, nmTaf, myTopic) == false)
//      throw new Exception("Something happened!!!");
//
//  await myMessagesSender(new List<string>()
//  {
//      "#اسئلة",
//      "الملف ال اسمه Installing_widows_server_2016_mcq-1"
//  });
// if (await sender.Send(myChat, iwsData.Skip(40), myTopic) == false)
//    throw new Exception("Something happened!!!");
//
// await myMessagesSender(new List<string>()
//  {
//      "#اسئلة",
//      "الملف ال اسمه Creating_Domain"
//  });
//  if (await sender.Send(myChat, cdData, myTopic) == false)
//      throw new Exception("Something happened!!!");
//
//  await myMessageSender(
//      "اضغطوا على هذا ال هاشتاج #اسئلة وتنقلوا.");

await myMessageSender(
    "شيئ اخير اطلب منكم ان تدعوا لصاحب هذا البوت بالهداية والتوفيق الي الخير, والثبات على الحق.");
Console.WriteLine();
return;

Func<List<string>, Task> MessagesSender(ITelegramBot bot, string chatId, int? topicId) => async (List<string> text) =>
{
    await bot.SendMsg(
        text.Select(x => new TelegramMsg() { chat_id = chatId, message_thread_id = topicId, text = x }));
};

Func<string, Task> MessageSender(ITelegramBot bot, string chatId, int? topicId) => async (string text) =>
{
    await bot.SendMsg(new TelegramMsg { chat_id = chatId, message_thread_id = topicId, text = text });
};
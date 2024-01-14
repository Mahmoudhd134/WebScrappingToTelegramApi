using System.Text;
using Newtonsoft.Json;
using ScrapingLibrary.Models.Quiz;
using ScrapingLibrary.Models.Telegram;
using ScrapingLibrary.Models.Telegram.Types;

namespace ScrapingLibrary.Implementation.Telegram;

public class TelegramBot : ITelegramBot, IQuizModelTelegramSender
{
    private readonly string _api;

    public TelegramBot(string api)
    {
        _api = api;
    }


    public async Task<bool> SendMsg(TelegramMsg telegramMsg)
    {
        if (string.IsNullOrWhiteSpace(telegramMsg.text.Trim()))
            return true;

        var json = JsonConvert.SerializeObject(telegramMsg);
        return await SendPostRequest(EndPoints.SendMessage, json);
    }

    public async Task<bool> SendMsg(IEnumerable<TelegramMsg> telegramMessages)
    {
        foreach (var telegramMsg in telegramMessages)
        {
            if (await SendMsg(telegramMsg) == false) return false;
            Thread.Sleep(4000);
        }

        return true;
    }

    public async Task<bool> SendPhoto(TelegramPhoto telegramPhoto)
    {
        var json = JsonConvert.SerializeObject(telegramPhoto);
        return await SendPostRequest(EndPoints.SendPhoto, json);
    }

    public async Task<bool> SendPhoto(IEnumerable<TelegramPhoto> telegramPhotos)
    {
        foreach (var telegramPhoto in telegramPhotos)
        {
            if (await SendPhoto(telegramPhoto) == false) return false;
            Thread.Sleep(4000);
        }

        return true;
    }

    public async Task<bool> SendQuiz(TelegramQuiz telegramQuiz)
    {
        var json = JsonConvert.SerializeObject(telegramQuiz);
        return await SendPostRequest(EndPoints.SendPoll, json);
    }

    public async Task<bool> SendQuiz(IEnumerable<TelegramQuiz> telegramQuizzes)
    {
        foreach (var telegramQuiz in telegramQuizzes)
        {
            if (await SendQuiz(telegramQuiz) == false) return false;
            Thread.Sleep(4000);
        }

        return true;
    }

    public async Task<bool> SendVote(TelegramVote telegramVote)
    {
        var json = JsonConvert.SerializeObject(telegramVote);
        return await SendPostRequest(EndPoints.SendPoll, json);
    }

    public async Task<bool> SendVote(IEnumerable<TelegramVote> telegramVotes)
    {
        foreach (var telegramVote in telegramVotes)
        {
            if (await SendVote(telegramVote) == false) return false;
            Thread.Sleep(4000);
        }

        return true;
    }


    public async Task<bool> Send(string chatId, QuizModel quizModel, int messageReplyId = default)
    {
        if (quizModel.ValidateQuiz() == QuizValidationTypes.Valid)
        {
            if (await SendQuiz(new TelegramQuiz
                {
                    chat_id = chatId,
                    question = quizModel.Question.Trim(),
                    options = quizModel.GetOptions(),
                    correct_option_id = quizModel.GetCorrectOptionId(),
                    is_anonymous = false,
                    message_thread_id = messageReplyId
                }) == false) return false;
        }
        else if (quizModel.ValidateQuiz() == QuizValidationTypes.LimitExceeded)
        {
            var telegramMessages = new List<string>
            {
                quizModel.Question.Trim() ?? "", quizModel.AnswerA ?? "", quizModel.AnswerB ?? "",
                quizModel.AnswerC ?? "",
                quizModel.AnswerD ?? "", quizModel.AnswerE ?? ""
            }.Select(m => new TelegramMsg
            {
                chat_id = chatId,
                text = m,
                message_thread_id = messageReplyId
            });

            if (await SendMsg(telegramMessages) == false) return false;

            if (await SendQuiz(new TelegramQuiz
                {
                    chat_id = chatId,
                    message_thread_id = messageReplyId,
                    question = "the prev question",
                    options = new string[] { "a", "b", "c", "d", "e" },
                    correct_option_id = quizModel.GetCorrectOptionId(),
                    is_anonymous = false
                }) == false) return false;
        }
        else if (quizModel.ValidateQuiz() == QuizValidationTypes.MultipleAnswers)
        {
            var telegramMessages = new List<string>
            {
                quizModel.Question.Trim() ?? "", quizModel.AnswerA ?? "", quizModel.AnswerB ?? "",
                quizModel.AnswerC ?? "",
                quizModel.AnswerD ?? "", quizModel.AnswerE ?? ""
            }.Select(m => new TelegramMsg
            {
                chat_id = chatId,
                message_thread_id = messageReplyId,
                text = m
            });

            if (await SendMsg(telegramMessages) == false) return false;

            if (await SendVote(new TelegramVote
                {
                    chat_id = chatId,
                    message_thread_id = messageReplyId,
                    question = "the prev question",
                    options = new string[] { "a", "b", "c", "d", "e" },
                    is_anonymous = false
                }) == false) return false;

            if (await SendMsg(new TelegramMsg
                {
                    chat_id = chatId,
                    message_thread_id = messageReplyId,
                    text = quizModel.RightAnswer
                }) == false) return false;
        }
        else if (quizModel.ValidateQuiz() == QuizValidationTypes.HasImages)
        {
            //todo

            var telegramMessages = new List<string>
            {
                quizModel.Question.Trim() ?? "", quizModel.AnswerA ?? "", quizModel.AnswerB ?? "",
                quizModel.AnswerC ?? "",
                quizModel.AnswerD ?? "", quizModel.AnswerE ?? ""
            }.Select(m => new TelegramMsg
            {
                chat_id = chatId,
                message_thread_id = messageReplyId,
                text = m
            });

            // if (await SendMsg(telegramMessages) == false) return false;
            foreach (var msg in telegramMessages)
            {
                if (string.IsNullOrWhiteSpace(msg.text.Trim()))
                    continue;
                if (msg.text.ToLower().Contains("http") == false)
                {
                    if (await SendMsg(msg) == false) return false;
                    Thread.Sleep(4000);
                    continue;
                }

                var url = msg.text.Split("\n")
                    .First(s => s.Trim().StartsWith("http"));
                msg.text = string.Join("\n", msg.text.Split("\n")
                    .Where(s => s.Trim().StartsWith("http") == false && string.IsNullOrWhiteSpace(s) == false));
                Thread.Sleep(2000);
                if (await SendPhoto(new TelegramPhoto
                    {
                        photo = url,
                        message_thread_id = messageReplyId,
                        chat_id = chatId
                    }) == false) return false;
                Thread.Sleep(4000);
                if (await SendMsg(msg) == false) return false;
                Thread.Sleep(2000);
            }

            if (await SendQuiz(new TelegramQuiz
                {
                    chat_id = chatId,
                    message_thread_id = messageReplyId,
                    question = "the prev question",
                    options = new string[] { "a", "b", "c", "d", "e" },
                    correct_option_id = quizModel.GetCorrectOptionId(),
                    is_anonymous = false
                }) == false) return false;
        }
        else if (quizModel.ValidateQuiz() == QuizValidationTypes.NotValid)
        {
            throw new ArgumentException("The Quiz Is Not a Valid Quiz");
        }

        Thread.Sleep(4000);
        if (string.IsNullOrWhiteSpace(quizModel.Explanation) == false)
            return await SendMsg(new TelegramMsg
            {
                chat_id = chatId,
                message_thread_id = messageReplyId,
                text = $"Explanation => {quizModel.Explanation}"
            });
        return true;
    }

    public async Task<bool> Send(string chatId, IEnumerable<QuizModel> quizModels, int messageReplyId = default)
    {
        var i = 0;
        foreach (var quiz in quizModels)
        {
            if (string.IsNullOrWhiteSpace(quiz.RightAnswer))
                continue;
            Console.Write($"\rQuiz {i} Done!! => {quiz.Question.Split(" ").First()}");
            if (i++ % 10 == 0)
            {
                var messages = new string[]
                    {
                        new string('-', 20),
                        "اللهم صلى و سلم على سيدنا محمد و على اله و اصحابه اجمعين",
                        new string('-', 20)
                    }
                    .Select(s => new TelegramMsg
                    {
                        chat_id = chatId,
                        message_thread_id = messageReplyId,
                        text = s
                    });
                if (await SendMsg(messages) == false) return false;
            }

            if (await Send(chatId, quiz, messageReplyId) == false)
                return false;

            Thread.Sleep(4000);
        }

        return true;
    }

    public async Task<bool> Send(string chatId, QuizGroup quizGroup, int messageReplyId = default)
    {
        if (await SendMsg(new TelegramMsg
            {
                chat_id = chatId,
                message_thread_id = messageReplyId,
                text = quizGroup.Title
            }) == false)
            return false;

        Thread.Sleep(4000);
        var messages = new string[]
            {
                new string('-', 20),
                "اللهم صلى و سلم على سيدنا محمد و على اله و اصحابه اجمعين",
                new string('-', 20)
            }
            .Select(s => new TelegramMsg
            {
                chat_id = chatId,
                message_thread_id = messageReplyId,
                text = s
            });
        if (await SendMsg(messages) == false) return false;
        return await Send(chatId, quizGroup.QuizModels, messageReplyId);
    }

    private async Task<bool> SendPostRequest(EndPoints endPoint, string json, int retryNum = 0, int maxRetries = 3)
    {
        if (retryNum >= maxRetries)
            return false;

        var dataContent = new StringContent(json, Encoding.UTF8, "application/json");
        var httpClint = new HttpClient();
        var response = await httpClint.PostAsync(_api + "/" + endPoint, dataContent);

        if (response.IsSuccessStatusCode) return true;

        Console.WriteLine($"\n{endPoint} Failed!!\n");
        Thread.Sleep(3000);
        return await SendPostRequest(endPoint, json, retryNum + 1, maxRetries);
    }
}
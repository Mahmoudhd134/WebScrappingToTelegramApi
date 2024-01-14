using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs;

public class NmMcq : Pdf<List<QuizModel>>
{
    public NmMcq(string path) : base(path)
    {
    }

    public override async Task<List<QuizModel>> Parse()
    {
        var text = await File.ReadAllTextAsync(Path);

        var quizzes = Extract(text).ToList();
        return quizzes;
    }

    private IEnumerable<QuizModel> Extract(string text)
    {
        var content = text
            .Split(" ")
            .SelectMany(x => x.Split("\n"))
            .Select(s => s.Trim())
            .Where(s => string.IsNullOrWhiteSpace(s) == false);

        var currentQuizNum = 0;
        var mode = StructForQuiz.Question;
        var quiz = new QuizModel();
        foreach (var word in content)
        {
            if (word.ToLower().StartsWith((currentQuizNum + 1).ToString() + '.'))
            {
                if (currentQuizNum != 0)
                {
                    // quiz.RightAnswer = "";
                    yield return quiz;
                    quiz = new QuizModel();
                }

                currentQuizNum++;
                mode = StructForQuiz.Question;
                quiz.Question = word;
            }
            else if (word.Contains("A)") && word.Length < 6)
            {
                mode = StructForQuiz.AnswerA;
                quiz.AnswerA = word;
            }
            else if (word.Contains("B)") && word.Length < 6)
            {
                mode = StructForQuiz.AnswerB;
                quiz.AnswerB = word;
            }
            else if (word.Contains("C)") && word.Length < 6)
            {
                mode = StructForQuiz.AnswerC;
                quiz.AnswerC = word;
            }
            else if (word.Contains("D)") && word.Length < 6)
            {
                mode = StructForQuiz.AnswerD;
                quiz.AnswerD = word;
            }
            else if (word.Contains("E)") && word.Length < 6)
            {
                mode = StructForQuiz.AnswerE;
                quiz.AnswerE = word;
            }
            else if (word.Contains("Answer:") && word.Length < 15)
            {
                mode = StructForQuiz.CorrectAnswer;
            }
            else
            {
                switch (mode)
                {
                    case StructForQuiz.Question:
                        quiz.Question += " " + word;
                        break;

                    case StructForQuiz.AnswerA:
                        quiz.AnswerA += " " + word;
                        break;

                    case StructForQuiz.AnswerB:
                        quiz.AnswerB += " " + word;
                        break;

                    case StructForQuiz.AnswerC:
                        quiz.AnswerC += " " + word;
                        break;

                    case StructForQuiz.AnswerD:
                        quiz.AnswerD += " " + word;
                        break;

                    case StructForQuiz.AnswerE:
                        quiz.AnswerE += " " + word;
                        break;

                    case StructForQuiz.CorrectAnswer:
                        quiz.RightAnswer = word.Trim().First().ToString();

                        yield return quiz;
                        quiz = new QuizModel();
                        mode = StructForQuiz.Question;
                        currentQuizNum++;
                        break;

                    default:
                        // throw new NotImplementedException("The line starts with un expected value");
                        break;
                }
            }
        }

        // quiz.RightAnswer = "";
        // yield return quiz;
    }
}
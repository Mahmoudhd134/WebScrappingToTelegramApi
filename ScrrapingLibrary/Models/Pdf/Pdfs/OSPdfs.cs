using System.Collections;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs;

public class OSPdfs : Pdf<IEnumerable<QuizModel>>
{
    public OSPdfs(string path) : base(path)
    {
    }

    public override Task<IEnumerable<QuizModel>> Parse()
    {
        return Task.FromResult(Extract());
    }

    private IEnumerable<QuizModel> Extract()
    {
        var pdfReader = new PdfReader(Path);
        var sb = new StringBuilder();
        for (var pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
            sb.AppendLine(PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber));

        var content = sb.ToString()
            .Split(" ")
            .SelectMany(s => s.Split("\n"))
            .Select(s => s.Trim())
            .Where(s => string.IsNullOrWhiteSpace(s) == false);

        var currentQuizNum = 0;
        var mode = StructForQuiz.Nothing;
        var quiz = new QuizModel();
        foreach (var word in content)
        {
            if (word.ToLower().StartsWith((currentQuizNum + 1).ToString() + ')') ||
                word.ToLower().StartsWith((currentQuizNum + 1).ToString() + ')') ||
                word.ToLower().StartsWith((currentQuizNum + 2).ToString() + ')') ||
                word.ToLower().StartsWith((currentQuizNum + 2).ToString() + ')') ||
                word.ToLower().StartsWith((currentQuizNum).ToString() + ')') ||
                word.ToLower().StartsWith((currentQuizNum).ToString() + ')'))
            {
                if (currentQuizNum != 0)
                {
                    // quiz.RightAnswer = "";
                    quiz.RightAnswer = GetAnswer(quiz);
                    yield return quiz;
                    quiz = new QuizModel();
                }

                currentQuizNum++;
                mode = StructForQuiz.Question;
                quiz.Question = word;
            }
            else if (word.ToLower().StartsWith("a)"))
            {
                mode = StructForQuiz.AnswerA;
                quiz.AnswerA = word;
            }
            else if (word.ToLower().StartsWith("b)"))
            {
                mode = StructForQuiz.AnswerB;
                quiz.AnswerB = word;
            }
            else if (word.ToLower().StartsWith("c)"))
            {
                mode = StructForQuiz.AnswerC;
                quiz.AnswerC = word;
            }
            else if (word.ToLower().StartsWith("d)"))
            {
                mode = StructForQuiz.AnswerD;
                quiz.AnswerD = word;
            }
            else if (word.ToLower().StartsWith("e)"))
            {
                mode = StructForQuiz.AnswerE;
                quiz.AnswerE = word;
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
                        quiz.RightAnswer = word;
                        break;

                    default:
                        // throw new NotImplementedException("The line starts with un expected value");
                        break;
                }
            }
        }

        // quiz.RightAnswer = "";
        quiz.RightAnswer = GetAnswer(quiz);
        yield return quiz;
    }

    private string GetAnswer(QuizModel quizModel)
    {
        if (quizModel.AnswerA?.Contains('*') ?? false)
        {
            quizModel.AnswerA = string.Join("",
                quizModel.AnswerA.Where(c => '*'.ToString().Equals(char.ToString(c)) == false));
            return "a";
        }

        if (quizModel.AnswerB?.Contains('*') ?? false)
        {
            quizModel.AnswerB = string.Join("",
                quizModel.AnswerB.Where(c => '*'.ToString().Equals(char.ToString(c)) == false));
            return "b";
        }

        if (quizModel.AnswerC?.Contains('*') ?? false)
        {
            quizModel.AnswerC = string.Join("",
                quizModel.AnswerC.Where(c => '*'.ToString().Equals(char.ToString(c)) == false));
            return "c";
        }

        if (quizModel.AnswerD?.Contains('*') ?? false)
        {
            quizModel.AnswerD = string.Join("",
                quizModel.AnswerD.Where(c => '*'.ToString().Equals(char.ToString(c)) == false));
            return "d";
        }

        if (quizModel.AnswerE?.Contains('*') ?? false)
        {
            quizModel.AnswerE = string.Join("",
                quizModel.AnswerE.Where(c => '*'.ToString().Equals(char.ToString(c)) == false));
            return "e";
        }

        throw new InvalidOperationException($"The Quiz Has No Right Answer, The Quiz => {quizModel}");
    }
}
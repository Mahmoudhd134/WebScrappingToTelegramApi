using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs
{
    public class DSS2MCQ:Pdf<IEnumerable<QuizModel>>
    {
        public DSS2MCQ(string path) : base(path)
        {
        }
        public override Task<IEnumerable<QuizModel>> Parse()
        {
            return Task.FromResult(ExtractQuizzes());
        }
        private IEnumerable<QuizModel> ExtractQuizzes()
        {
            var pdfReader = new PdfReader(Path);
            var mode = StructForQuiz.Unit;
            var currentUnit = 0;
            var currentQuizNum = 0;
            var quiz = new QuizModel();
            for (var pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
            {
                var text = PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber)
                    .Split("\n")
                    .Select(l => l.Trim());
                foreach (var line in text)
                {
                    if (line.ToLower().StartsWith("unit"))
                    {
                        mode = StructForQuiz.Unit;
                        currentUnit++;
                        currentQuizNum = 0;
                    }
                    else if (line.ToLower().StartsWith((currentQuizNum + 1).ToString()))
                    {
                        currentQuizNum++;
                        mode = StructForQuiz.Question;
                        quiz.Question = line;
                    }
                    else if (line.ToLower().StartsWith("a."))
                    {
                        mode = StructForQuiz.AnswerA;
                        quiz.AnswerA = line;
                    }
                    else if (line.ToLower().StartsWith("b."))
                    {
                        mode = StructForQuiz.AnswerB;
                        quiz.AnswerB = line;
                    }
                    else if (line.ToLower().StartsWith("c."))
                    {
                        mode = StructForQuiz.AnswerC;
                        quiz.AnswerC = line;
                    }
                    else if (line.ToLower().StartsWith("d."))
                    {
                        mode = StructForQuiz.AnswerD;
                        quiz.AnswerD = line;
                    }
                    else if (line.ToLower().StartsWith("answer:"))
                    {
                        mode = StructForQuiz.CorrectAnswer;
                        var correctAnswer = line.Split(" ").Last();
                        quiz.RightAnswer = correctAnswer.First().ToString();
                        quiz.Explanation = "";
                        Console.Write($"\rQuiz Number {currentQuizNum} has been Obtained");
                        yield return quiz;
                        quiz = new QuizModel();
                    }
                    else
                    {
                        switch (mode)
                        {
                            case StructForQuiz.Question:
                                quiz.Question += line;
                                break;

                            case StructForQuiz.AnswerA:
                                quiz.AnswerA += line;
                                break;

                            case StructForQuiz.AnswerB:
                                quiz.AnswerB += line;
                                break;

                            case StructForQuiz.AnswerC:
                                quiz.AnswerC += line;
                                break;

                            case StructForQuiz.AnswerD:
                                quiz.AnswerD += line;
                                break;

                            default:
                                // throw new NotImplementedException("The line starts with un expected value");
                                break;
                        }
                    }
                }
            }
        }
    }
}
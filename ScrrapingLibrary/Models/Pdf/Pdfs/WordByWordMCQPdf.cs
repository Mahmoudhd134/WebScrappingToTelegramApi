using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs
{
    public class WordByWordMcqPdf : Pdf<IEnumerable<QuizModel>>
    {
        public WordByWordMcqPdf(string path) : base(path)
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
                .Select(s => s.Trim())
                .Where(s => string.IsNullOrWhiteSpace(s) == false);

            var currentQuizNum = 0;
            var mode = StructForQuiz.Nothing;
            var quiz = new QuizModel();
            foreach (var word in content)
            {
                if (word.ToLower().StartsWith((currentQuizNum + 1).ToString() + '.') ||
                    word.ToLower().StartsWith((currentQuizNum + 1).ToString() + '-') ||
                    word.ToLower().StartsWith((currentQuizNum + 2).ToString() + '.') ||
                    word.ToLower().StartsWith((currentQuizNum + 2).ToString() + '-') ||
                    word.ToLower().StartsWith((currentQuizNum).ToString() + '.') ||
                    word.ToLower().StartsWith((currentQuizNum).ToString() + '-'))
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
                else if (word.ToLower().StartsWith("answer:"))
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
                            quiz.RightAnswer = word;
                            break;

                        default:
                            // throw new NotImplementedException("The line starts with un expected value");
                            break;
                    }
                }
            }

            // quiz.RightAnswer = "";
            yield return quiz;
        }
    }
}
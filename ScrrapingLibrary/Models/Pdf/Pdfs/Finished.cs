using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs
{
    public class Finished : Pdf<IEnumerable<QuizModel>>
    {
        public Finished(string path) : base(path)
        {
        }

        public override Task<IEnumerable<QuizModel>> Parse()
        {
            return Task.FromResult(Extract());
        }

        private IEnumerable<QuizModel> Extract()
        {
            var pdfReader = new PdfReader(Path);
            var mode = StructForQuiz.Nothing;
            var currentQuizNum = 0;
            var quiz = new QuizModel();
            StringBuilder sb = new();
            for (var pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                sb.AppendLine(PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber));

            var lines = sb.ToString()
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.ToLower().Equals("(choose the best answer)") == false &&
                            s.ToLower().StartsWith("selected answer:") == false &&
                            string.IsNullOrWhiteSpace(s) == false);

            foreach (var line in lines)
            {
                if (line.ToLower().StartsWith("question " + (currentQuizNum + 1).ToString() + " of 30"))
                {
                    if (mode == StructForQuiz.Explanation || mode == StructForQuiz.AnswerD)
                    {
                        yield return quiz;
                        quiz = new QuizModel();
                    }

                    currentQuizNum++;
                    mode = StructForQuiz.Question;
                    quiz.Question = line;
                }
                else if (line.StartsWith("A."))
                {
                    mode = StructForQuiz.AnswerA;
                    quiz.AnswerA = line;
                }
                else if (line.StartsWith("B."))
                {
                    mode = StructForQuiz.AnswerB;
                    quiz.AnswerB = line;
                }
                else if (line.StartsWith("C."))
                {
                    mode = StructForQuiz.AnswerC;
                    quiz.AnswerC = line;
                }
                else if (line.StartsWith("D."))
                {
                    mode = StructForQuiz.AnswerD;
                    quiz.AnswerD = line;
                }
                else if (line.StartsWith("E."))
                {
                    mode = StructForQuiz.AnswerE;
                    quiz.AnswerE = line;
                }
                else if (line.StartsWith("Correct answer:"))
                {
                    mode = StructForQuiz.CorrectAnswer;
                    var correctAnswer = line.Split(" ").Last().SkipLast(1).Last().ToString().ToLower();
                    quiz.RightAnswer = correctAnswer;
                }
                else if (line.Equals("Feedback"))
                {
                    mode = StructForQuiz.Explanation;
                    quiz.Explanation = line;
                }
                else
                {
                    switch (mode)
                    {
                        case StructForQuiz.Question:
                            quiz.Question += "\n" + line;
                            break;

                        case StructForQuiz.AnswerA:
                            quiz.AnswerA += "\n" + line;
                            break;

                        case StructForQuiz.AnswerB:
                            quiz.AnswerB += "\n" + line;
                            break;

                        case StructForQuiz.AnswerC:
                            quiz.AnswerC += "\n" + line;
                            break;

                        case StructForQuiz.AnswerD:
                            quiz.AnswerD += "\n" + line;
                            break;
                    
                        case StructForQuiz.AnswerE:
                            quiz.AnswerE += "\n" + line;
                            break;

                        case StructForQuiz.Explanation:
                            quiz.Explanation += "\n" + line;
                            break;

                        default:
                            // throw new NotImplementedException("The line starts with un expected value");
                            break;
                    }
                }
            }

            yield return quiz;
        }
    }
}
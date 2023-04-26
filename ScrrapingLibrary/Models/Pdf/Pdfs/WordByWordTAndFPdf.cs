using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs
{
    public class WordByWordTAndFPdf : Pdf<IEnumerable<QuizModel>>
    {
        public WordByWordTAndFPdf(string path) : base(path)
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
                quiz.AnswerA = "True";
                quiz.AnswerB = "False";

                if (word.ToLower().StartsWith((currentQuizNum + 1).ToString() + '.'))
                {
                    if (currentQuizNum != 0)
                    {
                        quiz.Question = string.Join(" ", quiz.Question.Split(" ")
                            .Where(s => s.ToLower().Equals("(true)") == false &&
                                        s.ToLower().Equals("(false)") == false));
                        yield return quiz;
                        quiz = new QuizModel();
                    }

                    currentQuizNum++;
                    mode = StructForQuiz.Question;
                    quiz.Question = word;
                }
                else if (word.ToLower().Contains("true"))
                {
                    mode = StructForQuiz.CorrectAnswer;
                    quiz.RightAnswer = "A";
                }
                else if (word.ToLower().Contains("false"))
                {
                    mode = StructForQuiz.CorrectAnswer;
                    quiz.RightAnswer = "B";
                }
                else
                {
                    switch (mode)
                    {
                        case StructForQuiz.Question:
                            quiz.Question += " " + word;
                            break;

                        default:
                            // throw new NotImplementedException("The line starts with un expected value");
                            break;
                    }
                }
            }

            quiz.AnswerA = "True";
            quiz.AnswerB = "False";
            yield return quiz;
        }
    }
}
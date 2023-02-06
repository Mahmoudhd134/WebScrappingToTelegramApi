using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ScrapingLibrary.Models.Quiz;

namespace ScrapingLibrary.Models.Pdf.Pdfs
{
    public class DSS1 : Pdf<IEnumerable<QuizModel>>
    {
        public DSS1(string path) : base(path)
        {
        }

        public override Task<IEnumerable<QuizModel>> Parse()
        {
            return Task.FromResult(ExtractQuizzes());
        }

        private IEnumerable<QuizModel> ExtractQuizzes()
        {
            var pdfReader = new PdfReader(Path);
            var currentQuizNum = 0;
            var quiz = new QuizModel();
            var found = false;
            for (var pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
            {
                var text = PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber)
                    .Split("\n")
                    .Select(l => l.Trim());
                foreach (var line in text)
                {
                    if (line.ToLower().StartsWith((currentQuizNum + 1).ToString()))
                    {
                        if (found)
                            yield return quiz;
                        found = false;
                        currentQuizNum++;
                        var words = line.Split(" ");
                        var complete = words
                            .Any(s => s.Equals("(True)") || s.Equals("(False)"));
                        if (complete)
                        {
                            Format(quiz, line);
                            yield return quiz;
                            quiz = new QuizModel();
                        }
                        else
                        {
                            quiz.Question = line;
                        }
                    }
                    else
                    {
                        var complete = line.Split(" ")
                            .Any(s => s.Equals("(True)") || s.Equals("(False)"));
                        if (complete)
                        {
                            Format(quiz, line);
                            yield return quiz;
                            quiz = new QuizModel();
                        }
                        else
                        {
                            quiz.Question += "\n" + line;
                        }
                    }
                }
            }
        }

        private void Format(QuizModel quiz, string line)
        {
            var words = line.Split(" ");
            var answer = words
                .Any(s => s.Equals("(True)"));

            var sb = new StringBuilder();
            var ansInx = 0;
            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Equals(answer ? "(True)" : "(False)"))
                {
                    ansInx = i;
                    break;
                }

                sb.Append(words[i]).Append(" ");
            }

            if (quiz.Question == null)
                quiz.Question = sb.ToString();
            else
                quiz.Question += "\n" + sb.ToString();

            quiz.Explanation = string.Join(" ", words.Skip(ansInx)) ?? null;
            quiz.AnswerA = "True";
            quiz.AnswerB = "False";
            quiz.RightAnswer = answer ? "a" : "b";
        }
    }
}
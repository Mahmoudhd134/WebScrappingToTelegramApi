namespace ScrapingLibrary.Models.Quiz
{
    public class QuizModel
    {
        public string Question { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string AnswerE { get; set; }
        public string RightAnswer { get; set; }
        public string Explanation { get; set; }

        public int GetCorrectOptionId()
        {
            return RightAnswer.ToLower() switch
            {
                "a" => 0,
                "b" => 1,
                "c" => 2,
                "d" => 3,
                "e" => 4,
                _ => throw new ArgumentException("No Rights Answer")
            };
        }

        public IEnumerable<string> GetOptions()
        {
            var options = new List<string>();

            if (string.IsNullOrWhiteSpace(AnswerA) == false)
                options.Add(AnswerA);

            options.Add(string.IsNullOrWhiteSpace(AnswerB) ? "---" : AnswerB);

            if (string.IsNullOrWhiteSpace(AnswerC) == false)
                options.Add(AnswerC);

            if (string.IsNullOrWhiteSpace(AnswerD) == false)
                options.Add(AnswerD);

            if (string.IsNullOrWhiteSpace(AnswerE) == false)
                options.Add(AnswerE);

            return options;
        }

        public QuizValidationTypes ValidateQuiz()
        {
            if (string.IsNullOrWhiteSpace(Question))
                return QuizValidationTypes.NotValid;

            if (RightAnswer?.Length > 1)
                return QuizValidationTypes.MultipleAnswers;

            if (Question?.Length > 300)
                return QuizValidationTypes.LimitExceeded;

            if (AnswerA?.Length > 100)
                return QuizValidationTypes.LimitExceeded;
            if (AnswerB?.Length > 100)
                return QuizValidationTypes.LimitExceeded;
            if (AnswerC?.Length > 100)
                return QuizValidationTypes.LimitExceeded;
            if (AnswerD?.Length > 100)
                return QuizValidationTypes.LimitExceeded;
            if (AnswerE?.Length > 100)
                return QuizValidationTypes.LimitExceeded;

            if (Question?.ToLower().Split("\n").Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;

            if (AnswerA?.ToLower().Split("\n")
                    .Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;
            if (AnswerB?.ToLower().Split("\n")
                    .Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;

            if (AnswerC?.ToLower().Split("\n")
                    .Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;

            if (AnswerD?.ToLower().Split("\n")
                    .Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;

            if (AnswerE?.ToLower().Split("\n")
                    .Any(s => s.StartsWith("https://") || s.StartsWith("http://")) ??
                false)
                return QuizValidationTypes.HasImages;

            return QuizValidationTypes.Valid;
        }

        public QuizModel RefactorQuiz(int? no = null)
        {
            var n = new QuizModel();
            var r = " " + (no != null ? no.ToString() : string.Empty);
            n.Question = $"Q{r}) " + Question?.Trim();
            n.AnswerA = "A) " + AnswerA?.Trim();
            n.AnswerB = "B) " + AnswerB?.Trim();
            n.AnswerC = "C) " + AnswerC?.Trim();
            n.AnswerD = "D) " + AnswerD?.Trim();
            n.AnswerE = "E) " + AnswerE?.Trim();
            n.RightAnswer = RightAnswer;
            n.Explanation = Explanation?.Trim();
            return n;
        }
    
        public override string ToString()
        {
            return "{" +
                   $"Qustion: {Question}, Answers: [{AnswerA}, {AnswerB}, {AnswerC}, {AnswerD}],RightAnswer: {RightAnswer}, " +
                   $"Explanation: {Explanation}" +
                   "}";
        }
    }
}
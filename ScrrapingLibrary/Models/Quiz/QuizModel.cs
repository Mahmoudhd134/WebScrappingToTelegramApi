using System.Diagnostics;
using ScrapingLibrary.Helpers;

namespace ScrapingLibrary.Models.Quiz;

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

    public static char GetCorrectOptionChar(int o)
    {
        return o switch
        {
            0 => 'a',
            1 => 'b',
            2 => 'c',
            3 => 'd',
            4 => 'e',
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

        return options.Select(x => x.Trim());
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

    public void Shuffle()
    {
        if (RightAnswer.Length != 1)
            return;

        if (AnswerA?.ToLower().StartsWith("a)") ?? false)
            AnswerA = string.Join("", AnswerA.Skip(2));
        if (AnswerA?.ToLower().StartsWith("-\ta)") ?? false)
            AnswerA = string.Join("", AnswerA.Skip(4));
        
        if (AnswerB?.ToLower().StartsWith("b)") ?? false)
            AnswerB = string.Join("", AnswerB.Skip(2));
        if (AnswerB?.ToLower().StartsWith("-\tb)") ?? false)
            AnswerB = string.Join("", AnswerB.Skip(4));
        
        if (AnswerC?.ToLower().StartsWith("c)") ?? false)
            AnswerC = string.Join("", AnswerC.Skip(2));
        if (AnswerC?.ToLower().StartsWith("-\tc)") ?? false)
            AnswerC = string.Join("", AnswerC.Skip(4));
        
        if (AnswerD?.ToLower().StartsWith("d)") ?? false)
            AnswerD = string.Join("", AnswerD.Skip(2));
        if (AnswerD?.ToLower().StartsWith("-\td)") ?? false)
            AnswerD = string.Join("", AnswerD.Skip(4));
        
        if (AnswerE?.ToLower().StartsWith("e)") ?? false)
            AnswerE = string.Join("", AnswerE.Skip(2));
        if (AnswerE?.ToLower().StartsWith("-\te)") ?? false)
            AnswerE = string.Join("", AnswerE.Skip(4));

        var aa = AnswerA;
        var ab = AnswerB;
        var ac = AnswerC;
        var ad = AnswerD;
        var ae = AnswerE;
        var answers = new List<string>() { aa, ab, ac, ad, ae };
        answers = answers.Where(x => string.IsNullOrWhiteSpace(x) == false).ToList();
        answers.Shuffle();
        answers.Add(null);
        answers.Add(null);
        answers.Add(null);

        string ra = null;
        if (RightAnswer == "A")
            ra = aa;
        else if (RightAnswer == "B")
            ra = ab;
        else if (RightAnswer == "C")
            ra = ac;
        else if (RightAnswer == "D")
            ra = ad;
        else if (RightAnswer == "E")
            ra = ae;

        AnswerA = answers[0];
        AnswerB = answers[1];
        AnswerC = answers[2];
        AnswerD = answers[3];
        AnswerE = answers[4];

        RightAnswer = ra == AnswerA ? "A" :
            ra == AnswerB ? "B" :
            ra == AnswerC ? "C" :
            ra == AnswerD ? "D" :
            ra == AnswerE ? "E" :
            throw new ArgumentException();
    }
}
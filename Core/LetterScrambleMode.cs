using LinguaQuest.Models;

namespace LinguaQuest.Core;

// УСПАДКУВАННЯ + ІНКАПСУЛЯЦІЯ: ScrambledLetters лише через private set
public class LetterScrambleMode : GameMode
{
    public string ScrambledLetters { get; private set; }

    public LetterScrambleMode(Word word) : base(word)
    {
        ModeName = "Анаграма";
        ScrambledLetters = ShuffleFisherYates(word.TargetText);
    }

    public override bool CheckAnswer(string input) =>
        string.Equals(input?.Trim(), CorrectAnswer, StringComparison.OrdinalIgnoreCase);

    private static string ShuffleFisherYates(string word)
    {
        var chars = word.ToCharArray();
        var rng = Random.Shared;

        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        var result = new string(chars);
        if (result.Equals(word, StringComparison.OrdinalIgnoreCase) && chars.Length > 1)
            (chars[0], chars[1]) = (chars[1], chars[0]);

        return new string(chars);
    }
}
using LinguaQuest.Models;

namespace LinguaQuest.Core;

// УСПАДКУВАННЯ: режим множинного вибору (2×2)
public class QuickFindMode : GameMode
{
    public IReadOnlyList<string> Options { get; }

    public QuickFindMode(Word word) : base(word)
    {
        ModeName = "Швидкий вибір";
        Options = word.Options?.Count >= 4
            ? word.Options
            : BuildFallbackOptions(word.TargetText);
    }

    public override bool CheckAnswer(string input) =>
        string.Equals(input?.Trim(), CorrectAnswer, StringComparison.OrdinalIgnoreCase);

    private static List<string> BuildFallbackOptions(string correct)
    {
        // мінімальний fallback, якщо в БД немає 4 варіантів
        return new List<string> { correct, "—", "—", "—" };
    }
}
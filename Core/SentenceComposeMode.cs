using LinguaQuest.Models;

namespace LinguaQuest.Core;

// УСПАДКУВАННЯ: вільне введення перекладу
public class SentenceComposeMode : GameMode
{
    public SentenceComposeMode(Word word) : base(word)
    {
        ModeName = "Вільна відповідь";
    }

    public override bool CheckAnswer(string input) =>
        !string.IsNullOrWhiteSpace(input) &&
        string.Equals(input.Trim(), CorrectAnswer, StringComparison.OrdinalIgnoreCase);
}
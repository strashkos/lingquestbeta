using LinguaQuest.Models;

namespace LinguaQuest.Core;

// АБСТРАКЦІЯ + ІНКАПСУЛЯЦІЯ: спільний контракт і захищені дані раунду
public abstract class GameMode
{
    protected readonly Word CurrentWord; // композиція: режим "має" Word на час раунду

    public string ModeName { get; protected set; } = string.Empty;
    public string QuestionText => CurrentWord.SourceText;
    public string Hint => CurrentWord.Hint;
    public string CorrectAnswer => CurrentWord.TargetText;
    public string WordId => CurrentWord.Id;

    protected GameMode(Word word) => CurrentWord = word;

    // ПОЛІМОРФІЗМ: кожен спадкоємець перевіряє відповідь по-своєму
    public abstract bool CheckAnswer(string input);
}
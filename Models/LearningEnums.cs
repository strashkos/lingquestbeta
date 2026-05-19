namespace LinguaQuest.Models;

/// <summary>Мови в домені (мапінг у MongoDB як int).</summary>
public enum LearningLanguage
{
    Ukrainian = 0,
    English = 1
}

/// <summary>CEFR-рівні (мапінг у MongoDB як int).</summary>
public enum LearningLevel
{
    A1 = 0,
    A2 = 1,
    B1 = 2
}

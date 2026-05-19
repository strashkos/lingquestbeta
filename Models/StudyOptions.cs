namespace LinguaQuest.Models;

/// <summary>Єдина мова навчання в застосунку — англійська.</summary>
public static class StudyOptions
{
    public const string LanguageCode = "English";
    public const string LanguageDisplay = "Англійська";

    public static LearningLanguage TargetLanguage => LearningLanguage.English;

    public static LearningLevel ParseLevel(string level) => level.ToUpperInvariant() switch
    {
        "A2" => LearningLevel.A2,
        "B1" => LearningLevel.B1,
        _ => LearningLevel.A1
    };

    public static string LevelLabel(LearningLevel level) => level switch
    {
        LearningLevel.A2 => "A2",
        LearningLevel.B1 => "B1",
        _ => "A1"
    };
}

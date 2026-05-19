namespace LinguaQuest.Models;

public class UserProfile
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarDataUri { get; set; }
    public string TargetLanguage { get; set; } = StudyOptions.LanguageCode;
    public string Level { get; set; } = "A1";
    public int TotalScore { get; set; }
    public int TotalAttempts { get; set; }
    public int CorrectAnswers { get; set; }
    public List<string> LearnedWordIds { get; set; } = new();
    public List<LevelProgressEntry> LevelProgress { get; set; } = new();
    public List<MistakeEntry> Mistakes { get; set; } = new();

    public string GetDisplayNameOrUsername(string username) =>
        string.IsNullOrWhiteSpace(DisplayName) ? username : DisplayName;

    public static string LevelKey(string level) => level.ToUpperInvariant();

    public LevelProgressEntry GetOrCreateLevel(string level)
    {
        level = level.ToUpperInvariant() switch { "A2" => "A2", "B1" => "B1", _ => "A1" };
        var entry = LevelProgress.FirstOrDefault(p =>
            LevelKey(p.Level) == LevelKey(level));
        if (entry != null) return entry;

        entry = new LevelProgressEntry
        {
            Language = StudyOptions.LanguageCode,
            Level = level
        };
        LevelProgress.Add(entry);
        return entry;
    }

    public LevelStatus GetLevelStatus(string level)
    {
        if (!IsLevelUnlocked(level)) return LevelStatus.Locked;
        var entry = LevelProgress.FirstOrDefault(p => LevelKey(p.Level) == LevelKey(level));
        return entry?.Status ?? LevelStatus.NotStarted;
    }

    public bool IsLevelUnlocked(string level) => level.ToUpperInvariant() switch
    {
        "A1" => true,
        "A2" => IsLevelFinished("A1"),
        "B1" => IsLevelFinished("A2"),
        _ => false
    };

    private bool IsLevelFinished(string level)
    {
        var entry = LevelProgress.FirstOrDefault(p => LevelKey(p.Level) == LevelKey(level));
        var status = entry?.Status ?? LevelStatus.NotStarted;
        return status is LevelStatus.Completed or LevelStatus.Skipped;
    }

    public static string? GetNextLevel(string current) => current.ToUpperInvariant() switch
    {
        "A1" => "A2",
        "A2" => "B1",
        _ => null
    };

    public void RegisterAttempt(bool isCorrect)
    {
        TotalAttempts++;
        if (isCorrect)
        {
            CorrectAnswers++;
            TotalScore += 50;
        }
        else
        {
            TotalScore = Math.Max(0, TotalScore - 10);
        }
    }

    public void RecordMistake(Word word, string userAnswer, string modeName, string level)
    {
        Mistakes.Insert(0, new MistakeEntry
        {
            WordId = word.Id,
            SourceText = word.SourceText,
            TargetText = word.TargetText,
            UserAnswer = userAnswer ?? string.Empty,
            ModeName = modeName,
            Language = StudyOptions.LanguageCode,
            Level = level,
            OccurredAtUtc = DateTime.UtcNow
        });

        if (Mistakes.Count > 100)
            Mistakes.RemoveRange(100, Mistakes.Count - 100);
    }
}

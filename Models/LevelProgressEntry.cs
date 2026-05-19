namespace LinguaQuest.Models;

public enum LevelStatus
{
    Locked = 0,
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Skipped = 4
}

public class LevelProgressEntry
{
    public string Language { get; set; } = StudyOptions.LanguageCode;
    public string Level { get; set; } = "A1";
    public LevelStatus Status { get; set; } = LevelStatus.NotStarted;
    public int WordsLearnedInLevel { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}

namespace LinguaQuest.Models;

public class MistakeEntry
{
    public string WordId { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public string TargetText { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public string ModeName { get; set; } = string.Empty;
    public string Level { get; set; } = "A1";
    public string Language { get; set; } = StudyOptions.LanguageCode;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}

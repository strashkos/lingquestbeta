using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinguaQuest.Models;

[BsonIgnoreExtraElements]
public class Word
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int SourceLanguage { get; set; } = (int)LearningLanguage.Ukrainian; // 0
    public int TargetLanguage { get; set; } = (int)LearningLanguage.English;
    public int Level { get; set; } = (int)LearningLevel.A1;

    public string SourceText { get; set; } = string.Empty;
    public string TargetText { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Hint { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();

    // Зручність для UI/сервісів (не зберігається в BSON, якщо не потрібно)
    [BsonIgnore]
    public LearningLanguage TargetLanguageEnum =>
        Enum.IsDefined(typeof(LearningLanguage), TargetLanguage)
            ? (LearningLanguage)TargetLanguage
            : LearningLanguage.English;

    [BsonIgnore]
    public LearningLevel LevelEnum =>
        Enum.IsDefined(typeof(LearningLevel), Level)
            ? (LearningLevel)Level
            : LearningLevel.A1;
}
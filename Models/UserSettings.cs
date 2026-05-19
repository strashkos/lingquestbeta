using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinguaQuest.Models;

public class UserSettings
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
    public int TargetLanguage { get; set; } = (int)LearningLanguage.English;
    public int Level { get; set; } = (int)LearningLevel.A1;
    public bool SoundEnabled { get; set; } = true;
}
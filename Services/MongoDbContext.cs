using MongoDB.Driver;
using LinguaQuest.Data;
using LinguaQuest.Models;

namespace LinguaQuest.Services;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<Word> Words =>
        _database.GetCollection<Word>("words");

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("users");

    public IMongoCollection<UserSettings> UserSettings =>
        _database.GetCollection<UserSettings>("userSettings");

    public async Task EnsureIndexesAsync()
    {
        var usersIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var usersIndex = new CreateIndexModel<User>(usersIndexKeys, new CreateIndexOptions { Unique = true });
        await Users.Indexes.CreateOneAsync(usersIndex);

        var wordsIndexKeys = Builders<Word>.IndexKeys
            .Ascending(w => w.TargetLanguage)
            .Ascending(w => w.Level);
        await Words.Indexes.CreateOneAsync(new CreateIndexModel<Word>(wordsIndexKeys));
    }

    public async Task SeedDataAsync()
    {
        // Видалити застарілі німецькі слова (колишній код мови = 2)
        await Words.DeleteManyAsync(w => w.TargetLanguage == 2);

        var expected = WordSeedData.All.Count;
        var englishCount = await Words.CountDocumentsAsync(
            Builders<Word>.Filter.Eq(w => w.TargetLanguage, (int)LearningLanguage.English));

        if (englishCount == expected)
            return;

        await Words.DeleteManyAsync(
            Builders<Word>.Filter.Eq(w => w.TargetLanguage, (int)LearningLanguage.English));
        await Words.InsertManyAsync(WordSeedData.All);
    }
}

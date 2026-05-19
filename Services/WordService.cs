using MongoDB.Driver;
using LinguaQuest.Models;

namespace LinguaQuest.Services;

public class WordService : IWordService
{
    private readonly MongoDbContext _context;

    public WordService(MongoDbContext context) => _context = context;

    public async Task<List<Word>> GetWordsAsync(
        LearningLanguage targetLanguage,
        LearningLevel level,
        int limit = 200)
    {
        var filter = Builders<Word>.Filter.And(
            Builders<Word>.Filter.Eq(w => w.TargetLanguage, (int)targetLanguage),
            Builders<Word>.Filter.Eq(w => w.Level, (int)level));

        return await _context.Words.Find(filter).Limit(limit).ToListAsync();
    }

    public async Task<List<Word>> GetWordsByIdsAsync(IEnumerable<string> ids)
    {
        var idList = ids.Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        if (idList.Count == 0) return new List<Word>();

        var filter = Builders<Word>.Filter.In(w => w.Id, idList);
        return await _context.Words.Find(filter).ToListAsync();
    }

    public async Task<int> CountWordsAsync(LearningLanguage targetLanguage, LearningLevel level)
    {
        var filter = Builders<Word>.Filter.And(
            Builders<Word>.Filter.Eq(w => w.TargetLanguage, (int)targetLanguage),
            Builders<Word>.Filter.Eq(w => w.Level, (int)level));

        return (int)await _context.Words.CountDocumentsAsync(filter);
    }

    public async Task<Word?> GetWordByIdAsync(string id) =>
        await _context.Words.Find(w => w.Id == id).FirstOrDefaultAsync();
}

using LinguaQuest.Models;

namespace LinguaQuest.Services;

public interface IWordService
{
    Task<List<Word>> GetWordsAsync(
        LearningLanguage targetLanguage,
        LearningLevel level,
        int limit = 200);

    Task<List<Word>> GetWordsByIdsAsync(IEnumerable<string> ids);

    Task<int> CountWordsAsync(LearningLanguage targetLanguage, LearningLevel level);

    Task<Word?> GetWordByIdAsync(string id);
}

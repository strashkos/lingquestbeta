using LinguaQuest.Models;

namespace LinguaQuest.Services;

public interface IUserService
{
    User? CurrentUser { get; }
    Task<User?> GetUserAsync(string username);
    Task<User?> GetUserByIdAsync(string id);
    Task<bool> RegisterAsync(string username, string password, string level);
    Task<bool> LoginAsync(string username, string password);
    Task RestoreSessionAsync();
    Task UpdateProgressAsync(string wordId, bool isCorrect, string? userAnswer, string modeName);
    Task<bool> UpdateProfileAsync(string displayName, string? avatarDataUri, string level);
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<bool> SkipLevelAsync(string level);
    Task<bool> SetActiveLevelAsync(string level);
    Task<bool> CompleteLevelIfReadyAsync(string level, int totalWordsInLevel);
    Task SaveCurrentUserAsync();
    Task LogoutAsync();
}

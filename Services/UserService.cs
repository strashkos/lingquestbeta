using MongoDB.Driver;
using LinguaQuest.Models;
using Microsoft.Extensions.Logging;

namespace LinguaQuest.Services;

public class UserService : IUserService
{
    private readonly MongoDbContext _context;
    private readonly UserSessionStore _session;
    private readonly ILogger<UserService> _logger;

    public User? CurrentUser { get; private set; }

    public UserService(MongoDbContext context, UserSessionStore session, ILogger<UserService> logger)
    {
        _context = context;
        _session = session;
        _logger = logger;
    }

    public async Task<User?> GetUserAsync(string username) =>
        await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task<User?> GetUserByIdAsync(string id) =>
        await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task RestoreSessionAsync()
    {
        if (CurrentUser != null) return;

        var userId = await _session.GetUserIdAsync();
        if (string.IsNullOrEmpty(userId)) return;

        CurrentUser = await GetUserByIdAsync(userId);
        if (CurrentUser != null)
            await NormalizeAndSaveIfNeededAsync(CurrentUser);
    }

    public async Task<bool> RegisterAsync(string username, string password, string level)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Register attempt with empty username or password");
            return false;
        }

        if (await _context.Users.Find(u => u.Username == username).AnyAsync())
            return false;

        var user = new User
        {
            Username = username.Trim(),
            PasswordHash = HashPassword(password),
            ProgressProfile = new UserProfile
            {
                DisplayName = username.Trim(),
                TargetLanguage = StudyOptions.LanguageCode,
                Level = NormalizeLevel(level)
            }
        };

        user.ProgressProfile.GetOrCreateLevel(NormalizeLevel(level)).Status = LevelStatus.InProgress;

        try
        {
            await _context.Users.InsertOneAsync(user);
        }
        catch (MongoWriteException mwx) when (mwx.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            _logger.LogWarning("Duplicate username during registration: {Username}", username);
            return false;
        }

        CurrentUser = user;
        await _session.SaveUserIdAsync(user.Id);
        return true;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var user = await GetUserAsync(username.Trim());
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return false;

        await NormalizeAndSaveIfNeededAsync(user);
        CurrentUser = user;
        await _session.SaveUserIdAsync(user.Id);
        return true;
    }

    public async Task UpdateProgressAsync(string wordId, bool isCorrect, string? userAnswer, string modeName)
    {
        if (CurrentUser == null) return;

        var profile = CurrentUser.ProgressProfile;
        profile.RegisterAttempt(isCorrect);

        var level = profile.Level;
        var levelEntry = profile.GetOrCreateLevel(level);
        levelEntry.Status = LevelStatus.InProgress;

        if (isCorrect && !profile.LearnedWordIds.Contains(wordId))
            profile.LearnedWordIds.Add(wordId);
        else if (!isCorrect && !string.IsNullOrWhiteSpace(userAnswer))
        {
            var word = await _context.Words.Find(w => w.Id == wordId).FirstOrDefaultAsync();
            if (word != null)
                profile.RecordMistake(word, userAnswer, modeName, level);
        }

        await RecountLevelProgressAsync(level);
        await SaveCurrentUserAsync();
    }

    public async Task<bool> UpdateProfileAsync(string displayName, string? avatarDataUri, string level)
    {
        if (CurrentUser == null) return false;

        var profile = CurrentUser.ProgressProfile;
        if (!string.IsNullOrWhiteSpace(displayName))
            profile.DisplayName = displayName.Trim();

        if (avatarDataUri != null)
        {
            if (avatarDataUri.Length > 500_000) return false;
            profile.AvatarDataUri = string.IsNullOrWhiteSpace(avatarDataUri) ? null : avatarDataUri;
        }

        profile.TargetLanguage = StudyOptions.LanguageCode;
        level = NormalizeLevel(level);
        if (profile.IsLevelUnlocked(level))
        {
            profile.Level = level;
            var entry = profile.GetOrCreateLevel(level);
            if (entry.Status == LevelStatus.NotStarted)
                entry.Status = LevelStatus.InProgress;
        }

        await SaveCurrentUserAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        if (CurrentUser == null || string.IsNullOrWhiteSpace(newPassword)) return false;
        if (!VerifyPassword(currentPassword, CurrentUser.PasswordHash)) return false;

        CurrentUser.PasswordHash = HashPassword(newPassword);
        await SaveCurrentUserAsync();
        return true;
    }

    public async Task<bool> SkipLevelAsync(string level)
    {
        if (CurrentUser == null) return false;
        level = NormalizeLevel(level);
        if (!CurrentUser.ProgressProfile.IsLevelUnlocked(level)) return false;

        var entry = CurrentUser.ProgressProfile.GetOrCreateLevel(level);
        entry.Status = LevelStatus.Skipped;
        entry.CompletedAtUtc = DateTime.UtcNow;

        var next = UserProfile.GetNextLevel(level);
        if (next != null)
        {
            CurrentUser.ProgressProfile.Level = next;
            CurrentUser.ProgressProfile.GetOrCreateLevel(next).Status = LevelStatus.InProgress;
        }

        await SaveCurrentUserAsync();
        return true;
    }

    public async Task<bool> SetActiveLevelAsync(string level)
    {
        if (CurrentUser == null) return false;
        level = NormalizeLevel(level);
        if (!CurrentUser.ProgressProfile.IsLevelUnlocked(level)) return false;

        CurrentUser.ProgressProfile.TargetLanguage = StudyOptions.LanguageCode;
        CurrentUser.ProgressProfile.Level = level;

        var entry = CurrentUser.ProgressProfile.GetOrCreateLevel(level);
        if (entry.Status == LevelStatus.NotStarted)
            entry.Status = LevelStatus.InProgress;

        await SaveCurrentUserAsync();
        return true;
    }

    public async Task<bool> CompleteLevelIfReadyAsync(string level, int totalWordsInLevel)
    {
        if (CurrentUser == null || totalWordsInLevel <= 0) return false;

        level = NormalizeLevel(level);
        var profile = CurrentUser.ProgressProfile;
        var lvlEnum = StudyOptions.ParseLevel(level);

        var levelIds = (await _context.Words.Find(w =>
            w.TargetLanguage == (int)StudyOptions.TargetLanguage &&
            w.Level == (int)lvlEnum).Project(w => w.Id).ToListAsync()).ToHashSet();

        var learned = profile.LearnedWordIds.Count(id => levelIds.Contains(id));
        if (learned < totalWordsInLevel) return false;

        var entry = profile.GetOrCreateLevel(level);
        entry.Status = LevelStatus.Completed;
        entry.WordsLearnedInLevel = learned;
        entry.CompletedAtUtc = DateTime.UtcNow;

        var next = UserProfile.GetNextLevel(level);
        if (next != null && profile.IsLevelUnlocked(next))
        {
            profile.Level = next;
            profile.GetOrCreateLevel(next).Status = LevelStatus.InProgress;
        }

        await SaveCurrentUserAsync();
        return true;
    }

    public async Task SaveCurrentUserAsync()
    {
        if (CurrentUser == null) return;
        await _context.Users.ReplaceOneAsync(u => u.Id == CurrentUser.Id, CurrentUser);
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        await _session.ClearAsync();
    }

    private async Task NormalizeAndSaveIfNeededAsync(User user)
    {
        var changed = NormalizeProfile(user.ProgressProfile);
        if (changed)
            await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    private static bool NormalizeProfile(UserProfile profile)
    {
        var changed = false;
        if (profile.TargetLanguage != StudyOptions.LanguageCode)
        {
            profile.TargetLanguage = StudyOptions.LanguageCode;
            changed = true;
        }

        foreach (var lp in profile.LevelProgress)
        {
            if (lp.Language != StudyOptions.LanguageCode)
            {
                lp.Language = StudyOptions.LanguageCode;
                changed = true;
            }
        }

        foreach (var m in profile.Mistakes)
        {
            if (m.Language != StudyOptions.LanguageCode)
            {
                m.Language = StudyOptions.LanguageCode;
                changed = true;
            }
        }

        return changed;
    }

    private async Task RecountLevelProgressAsync(string level)
    {
        if (CurrentUser == null) return;

        var lvlEnum = StudyOptions.ParseLevel(level);
        var ids = (await _context.Words.Find(w =>
            w.TargetLanguage == (int)StudyOptions.TargetLanguage &&
            w.Level == (int)lvlEnum).Project(w => w.Id).ToListAsync()).ToHashSet();

        var learned = CurrentUser.ProgressProfile.LearnedWordIds.Count(id => ids.Contains(id));
        CurrentUser.ProgressProfile.GetOrCreateLevel(level).WordsLearnedInLevel = learned;
    }

    private static string NormalizeLevel(string level) => level.ToUpperInvariant() switch
    {
        "A2" => "A2",
        "B1" => "B1",
        _ => "A1"
    };

    private static string HashPassword(string password)
    {
        const int iterations = 100_000;
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);
        using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
            password, salt, iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return string.Join('.', iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    private static bool VerifyPassword(string password, string stored)
    {
        try
        {
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;
            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);
            using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
                password, salt, iterations, System.Security.Cryptography.HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(hash.Length);
            return CryptographicEquals(computed, hash);
        }
        catch
        {
            return false;
        }
    }

    private static bool CryptographicEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }
}

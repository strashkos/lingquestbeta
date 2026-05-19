using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace LinguaQuest.Services;

public class UserSessionStore
{
    private const string UserIdKey = "lq_user_id";
    private readonly ProtectedLocalStorage _storage;

    public UserSessionStore(ProtectedLocalStorage storage) => _storage = storage;

    public async Task SaveUserIdAsync(string userId)
    {
        await _storage.SetAsync(UserIdKey, userId);
    }

    public async Task<string?> GetUserIdAsync()
    {
        try
        {
            var result = await _storage.GetAsync<string>(UserIdKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _storage.DeleteAsync(UserIdKey);
        }
        catch
        {
            // ignore
        }
    }
}

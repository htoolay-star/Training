using System.Collections.Concurrent;

namespace WebApp.Services.Auth;

public sealed record TokenEntry(string AccessToken, string RefreshToken);

public interface ITokenStore
{
    TokenEntry? Get(string sessionId);
    void Set(string sessionId, TokenEntry entry);
    void Remove(string sessionId);
    bool Exists(string sessionId);

    SemaphoreSlim GetRefreshLock(string sessionId);
}

public sealed class TokenStore : ITokenStore
{
    private readonly ConcurrentDictionary<string, TokenEntry> _tokens = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public TokenEntry? Get(string sessionId) =>
        _tokens.TryGetValue(sessionId, out var entry) ? entry : null;

    public void Set(string sessionId, TokenEntry entry) => _tokens[sessionId] = entry;

    public void Remove(string sessionId)
    {
        _tokens.TryRemove(sessionId, out _);
        if (_locks.TryRemove(sessionId, out var gate))
            gate.Dispose();
    }

    public bool Exists(string sessionId) => _tokens.ContainsKey(sessionId);

    public SemaphoreSlim GetRefreshLock(string sessionId) =>
        _locks.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
}

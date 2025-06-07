using Microsoft.Extensions.Caching.Distributed;

namespace MsSentinel.BanffProtect.Tests.Unit.Fakes;

internal class FakeDistributedCache : IDistributedCache
{
    private readonly Dictionary<string, byte[]> _cache = new();

    public byte[]? Get(string key)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        return Task.FromResult(_cache.GetValueOrDefault(key));
    }

    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        _cache[key] = value;

        return Task.CompletedTask;
    }
}
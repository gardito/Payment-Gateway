using Cache.Common;

namespace Cache.InMemory;

public class InMemoryCache : IIdempotencyCachingSystem
{
    private HashSet<Guid> _guidCache;
    
    public InMemoryCache()
    {
        _guidCache = new HashSet<Guid>();
    }

    public Task<bool> Contains(Guid id) => Task.FromResult(_guidCache.TryGetValue(id, out _));
    
    public Task Add(Guid item)
    {
        _guidCache.Add(item);
        return Task.CompletedTask;
    }
}
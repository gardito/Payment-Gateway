using Common;

namespace Cache.Common;

public interface IIdempotencyCachingSystem
{
    public Task<bool> Contains(Guid id);
    public Task Add(Guid item);
}
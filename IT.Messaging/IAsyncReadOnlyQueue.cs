using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncReadOnlyQueue : IAsyncMemoryReadOnlyQueue
{
    Task<long> GetPositionAsync<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);

    Task<T?> GetByIndexAsync<T>(long index, string? queue = null);

    Task<T[]> GetRangeAsync<T>(long min = 0, long max = -1, string? queue = null);
}
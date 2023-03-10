using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncReadOnlyQueue : IAsyncMemoryReadOnlyQueue
{
    Task<long> GetPositionAsync<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);

    Task<T[]> GetRangeAsync<T>(long start = 0, long stop = -1, string? queue = null);
}
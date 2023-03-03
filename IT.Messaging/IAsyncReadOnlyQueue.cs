using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncReadOnlyQueue : IAsyncMemoryReadOnlyQueue
{
    Task<long> GetPositionAsync<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);
}
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueue : IAsyncMemoryQueue, IAsyncReadOnlyQueue, IAsyncChannel
{
    Task<long> DeleteAsync<T>(T message, long count = 0, string? queue = null);
}
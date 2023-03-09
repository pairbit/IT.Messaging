using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncDelayedQueue : IAsyncReadOnlyDelayedQueue, IAsyncMemoryDelayedQueue, IAsyncDelayedPublisher
{
    Task<bool> DeleteAsync<T>(T message, string? queue = null);
}
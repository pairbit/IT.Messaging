using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncReadOnlyQueue<T> : IAsyncQueueInformer
{
    Task<long> GetPositionAsync(T message, long rank = 1, long maxLength = 0, string? queue = null);
}
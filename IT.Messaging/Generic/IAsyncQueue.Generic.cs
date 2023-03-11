using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncQueue<T> : IAsyncChannel<T>, IAsyncReadOnlyQueue<T>, IAsyncQueueTrimmer, IAsyncQueueCleaner
{
    Task<long> DeleteAsync(T message, long count = 0, string? queue = null);
}
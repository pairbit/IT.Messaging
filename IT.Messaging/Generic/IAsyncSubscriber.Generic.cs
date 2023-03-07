using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncSubscriber<T> : IAsyncUnsubscriber
{
    Task SubscribeAsync(AsyncHandler<T>? handler, AsyncBatchHandler<T>? batchHandler = null, string? key = null);
}
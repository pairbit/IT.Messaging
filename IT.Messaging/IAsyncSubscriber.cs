using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncSubscriber : IAsyncMemorySubscriber
{
    Task SubscribeAsync<T>(AsyncHandler<T>? handler, AsyncBatchHandler<T>? batchHandler = null, string? key = null);
}
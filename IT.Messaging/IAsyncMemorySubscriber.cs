using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemorySubscriber : IAsyncUnsubscriber
{
    Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? key = null);

    //Task SubscribeAsync(AsyncMemoryHandler handler, AsyncSequenceHandler batchHandler, string? key = null);
}
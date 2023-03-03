namespace IT.Messaging;

public interface IQueue : IAsyncQueue, IMemoryQueue, IReadOnlyQueue, IChannel
{
    long Delete<T>(T message, long count = 0, string? queue = null);
}
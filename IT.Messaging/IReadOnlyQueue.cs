namespace IT.Messaging;

public interface IReadOnlyQueue : IAsyncReadOnlyQueue, IMemoryReadOnlyQueue
{
    long GetPosition<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);
}
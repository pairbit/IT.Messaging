namespace IT.Messaging;

public interface IPublisher : IAsyncPublisher, IMemoryPublisher
{
    long Publish<T>(T message, string? channel = null);

    long Publish<T>(T[] messages, string? channel = null);
}
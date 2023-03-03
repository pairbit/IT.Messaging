namespace IT.Messaging;

public interface IPublisher : IAsyncPublisher, IMemoryPublisher
{
    long Publish<T>(T message, string? key = null);

    long Publish<T>(T[] messages, string? key = null);
}
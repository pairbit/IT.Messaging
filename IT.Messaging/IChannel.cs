namespace IT.Messaging;

public interface IChannel : IAsyncChannel, IMemoryChannel, IPublisher, ISubscriber
{

}
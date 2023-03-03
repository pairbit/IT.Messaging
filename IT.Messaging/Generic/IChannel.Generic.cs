namespace IT.Messaging.Generic;

public interface IChannel<T> : IAsyncChannel<T>, IPublisher<T>, ISubscriber<T>
{

}
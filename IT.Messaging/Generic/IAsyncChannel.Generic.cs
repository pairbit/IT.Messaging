namespace IT.Messaging.Generic;

public interface IAsyncChannel<T> : IAsyncPublisher<T>, IAsyncSubscriber<T>
{

}
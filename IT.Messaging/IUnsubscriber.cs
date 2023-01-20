namespace IT.Messaging;

public interface IUnsubscriber : IAsyncUnsubscriber
{
    void UnsubscribeAll();

    void Unsubscribe(string channel);
}
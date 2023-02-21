using StackExchange.Redis;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class PubSubSubscriber
{
    protected readonly RedisChannel ChannelDefault = "*";
    protected readonly StackExchange.Redis.ISubscriber _subscriber;
    protected readonly IRedisValueDeserializer _deserializer;

    public PubSubSubscriber(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueDeserializer deserializer)
    {
        _subscriber = subscriber;
        _deserializer = deserializer;
    }

    public Task UnsubscribeAllAsync() => _subscriber.UnsubscribeAllAsync();

    public Task UnsubscribeAsync(string channel) => _subscriber.UnsubscribeAsync(channel);

    public void UnsubscribeAll() => _subscriber.UnsubscribeAll();

    public void Unsubscribe(string channel) => _subscriber.Unsubscribe(channel);
}
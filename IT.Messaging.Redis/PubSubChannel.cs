using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public abstract class PubSubChannel
{
    private const RedisChannel.PatternMode Mode = RedisChannel.PatternMode.Auto;
    protected readonly RedisChannel ChannelDefault = "*";
    protected readonly Encoding _encoding;
    protected readonly StackExchange.Redis.ISubscriber _subscriber;
    protected readonly IRedisValueSerializer _serializer;
    protected readonly IRedisValueDeserializer _deserializer;

    public PubSubChannel(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueSerializer serializer,
        IRedisValueDeserializer deserializer,
        Encoding? encoding = null)
    {
        _subscriber = subscriber;
        _serializer = serializer;
        _deserializer = deserializer;
        _encoding = encoding ?? Encoding.UTF8;
    }

    public Boolean IsConnected(string? channel = null) => _subscriber.IsConnected(channel is not null ? (RedisChannel)channel : default);

    public Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? channel)
        => _subscriber.PublishAsync(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), message);

    public Task<long> PublishAsync<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _subscriber.PublishAsync(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), redisValue);
    }

    public long Publish(ReadOnlyMemory<byte> message, string? channel)
        => _subscriber.Publish(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), message);

    public long Publish<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _subscriber.Publish(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), redisValue);
    }

    public Task UnsubscribeAllAsync() => _subscriber.UnsubscribeAllAsync();

    public Task UnsubscribeAsync(string channel) => _subscriber.UnsubscribeAsync(channel);

    public void UnsubscribeAll() => _subscriber.UnsubscribeAll();

    public void Unsubscribe(string channel) => _subscriber.Unsubscribe(channel);
}
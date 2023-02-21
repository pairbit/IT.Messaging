using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class PubSubPublisher : IPublisher
{
    private const RedisChannel.PatternMode Mode = RedisChannel.PatternMode.Auto;
    protected readonly RedisChannel ChannelDefault = "*";
    protected readonly Encoding _encoding;
    protected readonly StackExchange.Redis.ISubscriber _subscriber;
    protected readonly IRedisValueSerializer _serializer;

    public PubSubPublisher(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueSerializer serializer,
        Encoding? encoding = null)
    {
        _subscriber = subscriber;
        _serializer = serializer;
        _encoding = encoding ?? Encoding.UTF8;
    }

    //public Boolean IsConnected(string? channel = null) => _subscriber.IsConnected(channel is not null ? (RedisChannel)channel : default);

    public Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? channel)
        => _subscriber.PublishAsync(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), message);

    public Task<long> PublishAsync(ReadOnlyMemory<byte>[] messages, string? channel) => throw new NotSupportedException();

    public Task<long> PublishAsync<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _subscriber.PublishAsync(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), redisValue);
    }

    public Task<long> PublishAsync<T>(T[] messages, string? channel) => throw new NotSupportedException();

    public long Publish(ReadOnlyMemory<byte> message, string? channel)
        => _subscriber.Publish(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), message);

    public long Publish(ReadOnlyMemory<byte>[] messages, string? channel) => throw new NotSupportedException();

    public long Publish<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _subscriber.Publish(channel == null ? default : new RedisChannel(_encoding.GetBytes(channel), Mode), redisValue);
    }

    public long Publish<T>(T[] messages, string? channel) => throw new NotSupportedException();
}
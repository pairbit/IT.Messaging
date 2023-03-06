using IT.Messaging.Generic;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IT.Messaging.Redis.Generic;

public class PubSubPublisher<T> : IPublisher<T>
{
    private const RedisChannel.PatternMode Mode = RedisChannel.PatternMode.Auto;
    protected readonly RedisChannel ChannelDefault = "*";
    protected readonly StackExchange.Redis.ISubscriber _subscriber;
    protected readonly IRedisValueSerializer<T> _serializer;
    protected readonly Encoding _encoding;

    public PubSubPublisher(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueSerializer<T> serializer,
        Encoding? encoding = null)
    {
        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _encoding = encoding ?? Encoding.UTF8;
    }

    public long Publish(T message, string? key = null)
    {
        try
        {
            return _subscriber.Publish(GetChannel(key), _serializer.Serialize(message));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Publish(T[] messages, string? key = null)
    {
        try
        {
            return _subscriber.Publish(GetChannel(key), _serializer.Serialize(messages));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync(T message, string? key = null)
    {
        try
        {
            return _subscriber.PublishAsync(GetChannel(key), _serializer.Serialize(message));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync(T[] messages, string? key = null)
    {
        try
        {
            return _subscriber.PublishAsync(GetChannel(key), _serializer.Serialize(messages));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    protected RedisChannel GetChannel(string? key) => key == null ? default : new RedisChannel(_encoding.GetBytes(key), Mode);
}
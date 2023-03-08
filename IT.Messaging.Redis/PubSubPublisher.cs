using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class PubSubPublisher : PubSubMemoryPublisher, IPublisher
{
    protected readonly IRedisValueSerializer _serializer;

    public PubSubPublisher(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueSerializer serializer,
        Encoding? encoding = null) : base(subscriber, encoding)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public long Publish<T>(T message, string? key = null)
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

    public long Publish<T>(IEnumerable<T> messages, string? key = null)
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

    public Task<long> PublishAsync<T>(T message, string? key = null)
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

    public Task<long> PublishAsync<T>(IEnumerable<T> messages, string? key = null)
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
}
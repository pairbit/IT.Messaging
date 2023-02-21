using IT.Messaging.Redis.Internal;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class ListRightPublisher : IPublisher
{
    private readonly IDatabase _db;
    private readonly IRedisValueSerializer _serializer;

    public ListRightPublisher(IDatabase db, IRedisValueSerializer serializer)
    {
        _db = db;
        _serializer = serializer;
    }

    public Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? channel)
        => _db.ListRightPushAsync(GetKey(channel), message);

    public Task<long> PublishAsync(ReadOnlyMemory<byte>[] messages, string? channel)
        => _db.ListRightPushAsync(GetKey(channel), messages.To(x => (RedisValue)x));

    public Task<long> PublishAsync<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _db.ListRightPushAsync(GetKey(channel), redisValue);
    }

    public Task<long> PublishAsync<T>(T[] messages, string? channel)
        => _db.ListRightPushAsync(GetKey(channel), messages.To(x =>
        {
            _serializer.Serialize(x, out var redisValue);
            return redisValue;
        }));

    public long Publish(ReadOnlyMemory<byte> message, string? channel) => _db.ListRightPush(GetKey(channel), message);

    public long Publish(ReadOnlyMemory<byte>[] messages, string? channel) => _db.ListRightPush(GetKey(channel), messages.To(x => (RedisValue)x));

    public long Publish<T>(T message, string? channel)
    {
        _serializer.Serialize(message, out var redisValue);
        return _db.ListRightPush(GetKey(channel), redisValue);
    }

    public long Publish<T>(T[] messages, string? channel)
        => _db.ListRightPush(GetKey(channel), messages.To(x =>
        {
            _serializer.Serialize(x, out var redisValue);
            return redisValue;
        }));

    private RedisKey GetKey(string? channel)
    {
        return channel ?? string.Empty;
    }
}
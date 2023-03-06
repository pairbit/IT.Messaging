using StackExchange.Redis;

namespace IT.Messaging.Redis.Generic;

public interface IRedisValueSerializer<T>
{
    RedisValue Serialize(T value);

    RedisValue Serialize(T[] values);
}
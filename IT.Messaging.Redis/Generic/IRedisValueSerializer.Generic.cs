using StackExchange.Redis;
using System.Collections.Generic;

namespace IT.Messaging.Redis.Generic;

public interface IRedisValueSerializer<T>
{
    RedisValue Serialize(T value);

    RedisValue Serialize(IEnumerable<T> values);
}
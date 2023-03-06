using StackExchange.Redis;

namespace IT.Messaging.Redis;

public interface IRedisValueSerializer
{
    RedisValue Serialize<T>(T value);
}
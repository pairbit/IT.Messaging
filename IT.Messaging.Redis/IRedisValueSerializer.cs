using StackExchange.Redis;

namespace IT.Messaging.Redis;

public interface IRedisValueSerializer
{
    void Serialize<T>(T value, out RedisValue redisValue);
}
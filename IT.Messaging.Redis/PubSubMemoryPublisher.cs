using StackExchange.Redis;
using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class PubSubMemoryPublisher : IMemoryPublisher
{
    private const RedisChannel.PatternMode Mode = RedisChannel.PatternMode.Auto;
    protected readonly RedisChannel ChannelDefault = "*";
    protected readonly StackExchange.Redis.ISubscriber _subscriber;
    protected readonly Encoding _encoding;

    public PubSubMemoryPublisher(
        StackExchange.Redis.ISubscriber subscriber,
        Encoding? encoding = null)
    {
        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        _encoding = encoding ?? Encoding.UTF8;
    }

    public long Publish(ReadOnlyMemory<byte> message, string? key = null)
    {
        try
        {
            return _subscriber.Publish(GetChannel(key), message);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Publish(ReadOnlyMemory<byte>[] messages, string? key = null)
    {
        //есть два способа публиковать пачку сообщений в канал:
        //1) если все сообщения одинаковой длинны, длина указывается в конфигурации и проверяется пеерд отправкой
        //2) если сообщения переменной длины, то создаем массив байтов длинной в сумму всех сообщений и плюс количество сообщений умноженное на 4 байта (по одному байту на длинну сообщения)

        throw new NotImplementedException();
    }

    public long Publish(in ReadOnlySequence<byte> messages, string? key = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? key = null)
    {
        try
        {
            return _subscriber.PublishAsync(GetChannel(key), message);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync(ReadOnlyMemory<byte>[] messages, string? key = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> PublishAsync(in ReadOnlySequence<byte> messages, string? key = null)
    {
        throw new NotImplementedException();
    }

    protected RedisChannel GetChannel(string? key) => key == null ? default : new RedisChannel(_encoding.GetBytes(key), Mode);
}
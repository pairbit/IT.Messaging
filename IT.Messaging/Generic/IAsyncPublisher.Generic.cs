using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncPublisher<T>
{
    Task<long> PublishAsync(T message, string? key = null);

    Task<long> PublishAsync(T[] messages, string? key = null);
}
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncPublisher<T>
{
    Task<long> PublishAsync(T message, string? channel = null);

    Task<long> PublishAsync(T[] messages, string? channel = null);
}
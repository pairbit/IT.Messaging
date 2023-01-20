using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncPublisher<T>
{
    Task<long> PublishAsync(T message, string? channel = null);
}
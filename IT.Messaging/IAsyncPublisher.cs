using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncPublisher : IAsyncMemoryPublisher
{
    Task<long> PublishAsync<T>(T message, string? channel = null);

    Task<long> PublishAsync<T>(T[] messages, string? channel = null);
}
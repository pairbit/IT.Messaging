using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncPublisher : IAsyncMemoryPublisher
{
    Task<long> PublishAsync<T>(T message, string? key = null);

    Task<long> PublishAsync<T>(T[] messages, string? key = null);
}
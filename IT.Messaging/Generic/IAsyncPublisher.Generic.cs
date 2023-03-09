using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncPublisher<T>
{
    Task<long> PublishAsync(T message, string? key = null);

    Task<long> PublishAsync(IEnumerable<T> messages, string? key = null);
}
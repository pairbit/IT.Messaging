using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncSubscriber : IAsyncMemorySubscriber
{
    Task SubscribeAsync<T>(Action<T, string?> handler, string? key = null);

    Task SubscribeAsync<T>(Action<T[], string?> handler, string? key = null);
}
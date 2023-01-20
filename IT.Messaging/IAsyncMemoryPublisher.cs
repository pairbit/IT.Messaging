using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryPublisher
{
    Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? channel = null);
}
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueInformer
{
    Task<long> GetLengthAsync(string? queue = null);
}
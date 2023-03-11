using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueTrimmer
{
    Task TrimAsync(long min, long max, string? queue = null);
}
using System.Threading;
using System.Threading.Tasks;

namespace IT.Messaging.Redis.Internal;

internal class Tasks
{
    private readonly Task[] _array;
    private readonly CancellationTokenSource _tokenSource;

    public Task[] Array => _array;

    public CancellationTokenSource TokenSource => _tokenSource;

    public Tasks(Task[] array, CancellationTokenSource tokenSource) 
    {
        _array = array;
        _tokenSource = tokenSource;
    }
}
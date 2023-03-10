using StackExchange.Redis;
using IT.Messaging.Redis.Internal;
using IT.Messaging.Redis;
using System.Text;

namespace IT.Messaging.Tests;

public class MemoryQueueTest
{
    private IMemoryQueue _queue;

    [SetUp]
    public void Setup()
    {
        var conn = ConnectionMultiplexer.Connect("localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");

        var db = conn.GetDatabase();

        _queue = new MemoryQueue(db, NotImplementedMemorySubscriber.Default);
    }

    [Test]
    public void ListRemoveAllTest1()
    {
        _queue.Clean();

        Assert.That(_queue.Publish(GetMessage(0)), Is.EqualTo(1));

        Assert.That(_queue.Publish(GetMessageEnumerable(1, 2)), Is.EqualTo(3));

        Assert.That(_queue.Publish(GetMessageArray(3, 2)), Is.EqualTo(5));

        Assert.That(_queue.Delete(GetMessage(0)), Is.EqualTo(1));

        //Assert.That(_queue.Delete(GetMessageArray(1, 2, 3)), Is.EqualTo(4));

        Assert.That(_queue.Delete(GetMessageEnumerable(1, 2, 3)), Is.EqualTo(4));

        Assert.False(_queue.Exists());
    }

    private static ReadOnlyMemory<byte> GetMessage(string key) => Encoding.UTF8.GetBytes(key).AsMemory();

    private static ReadOnlyMemory<byte> GetMessage(int key) => BitConverter.GetBytes(key).AsMemory();

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageEnumerable(params int[] keys)
        => keys.Select(x => (ReadOnlyMemory<byte>)BitConverter.GetBytes(x).AsMemory());

    private static ReadOnlyMemory<byte>[] GetMessageArray(params int[] keys) => GetMessageEnumerable(keys).ToArray();
}
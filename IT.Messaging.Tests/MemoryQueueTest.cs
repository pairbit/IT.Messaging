using StackExchange.Redis;
using IT.Messaging.Redis.Internal;
using IT.Messaging.Redis;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

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

        //Assert.That(_queue.Delete(GetMessageList(1, 2, 3)), Is.EqualTo(4));

        //Assert.That(_queue.Delete(GetMessageReadOnlyCollection(1, 2, 3)), Is.EqualTo(4));

        //Assert.That(_queue.Delete(GetMessageCollectionGeneric(1, 2, 3)), Is.EqualTo(4));

        //Assert.That(_queue.Delete(GetMessageCollection(1, 2, 3)), Is.EqualTo(4));

        Assert.That(_queue.Delete(GetMessageEnumerable(1, 2, 3)), Is.EqualTo(4));

        Assert.False(_queue.Exists());
    }

    private static ReadOnlyMemory<byte> GetMessage(string key) => Encoding.UTF8.GetBytes(key).AsMemory();

    private static ReadOnlyMemory<byte> GetMessage(int key) => BitConverter.GetBytes(key).AsMemory();

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageEnumerable(params int[] keys)
        => keys.Select(x => (ReadOnlyMemory<byte>)BitConverter.GetBytes(x).AsMemory());

    private static ReadOnlyMemory<byte>[] GetMessageArray(params int[] keys) => GetMessageEnumerable(keys).ToArray();

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageList(params int[] keys) => new MemoryList(GetMessageEnumerable(keys).ToList());

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageReadOnlyCollection(params int[] keys) => new MemoryReadOnlyCollection(GetMessageEnumerable(keys).ToList());

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageCollectionGeneric(params int[] keys) => new MemoryCollectionGeneric(GetMessageEnumerable(keys).ToList());

    private static IEnumerable<ReadOnlyMemory<byte>> GetMessageCollection(params int[] keys) => new MemoryCollection(GetMessageEnumerable(keys).ToList());

    private class MemoryList : IList<ReadOnlyMemory<byte>>
    {
        private readonly List<ReadOnlyMemory<byte>> _list;

        public MemoryList(List<ReadOnlyMemory<byte>> list)
        {
            _list = list;
        }

        public ReadOnlyMemory<byte> this[int index] { get => _list[index]; set => _list[index] = value; }

        public int Count => _list.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ReadOnlyMemory<byte>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ReadOnlyMemory<byte>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    private class MemoryReadOnlyCollection : IReadOnlyCollection<ReadOnlyMemory<byte>>
    {
        private readonly List<ReadOnlyMemory<byte>> _list;

        public MemoryReadOnlyCollection(List<ReadOnlyMemory<byte>> list)
        {
            _list = list;
        }

        public int Count => _list.Count;

        public IEnumerator<ReadOnlyMemory<byte>> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    private class MemoryCollectionGeneric : ICollection<ReadOnlyMemory<byte>>
    {
        private readonly List<ReadOnlyMemory<byte>> _list;

        public MemoryCollectionGeneric(List<ReadOnlyMemory<byte>> list)
        {
            _list = list;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ReadOnlyMemory<byte>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ReadOnlyMemory<byte>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(ReadOnlyMemory<byte> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    private class MemoryCollection : ICollection, IEnumerable<ReadOnlyMemory<byte>>
    {
        private readonly List<ReadOnlyMemory<byte>> _list;

        public MemoryCollection(List<ReadOnlyMemory<byte>> list)
        {
            _list = list;
        }

        public int Count => _list.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<ReadOnlyMemory<byte>> IEnumerable<ReadOnlyMemory<byte>>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
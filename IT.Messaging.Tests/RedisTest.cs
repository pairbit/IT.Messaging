using StackExchange.Redis;

namespace IT.Messaging.Tests;

public class RedisTest
{
    private IDatabase _db;

    [SetUp]
    public void Setup()
    {
        var conn = ConnectionMultiplexer.Connect("localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");

        _db = conn.GetDatabase();
    }

    [Test]
    public void ListTest()
    {
        RedisKey queue = "q1";
        RedisKey queueWorking = "q1-working";

        var queueSize = 9;
        var batchSize = 3;

        _db.KeyDelete(new[] { queue, queueWorking });

        for (int i = 0; i < queueSize; i++)
        {
            Assert.That(_db.ListLeftPush(queue, i), Is.EqualTo(i + 1));
        }

        Assert.That(_db.ListLength(queue), Is.EqualTo(queueSize));

        try
        {
            for (int i = 0; i < batchSize; i++)
            {
                Assert.That((long)_db.ListMove(queue, queueWorking, ListSide.Right, ListSide.Left), Is.EqualTo(i));
            }

            for (int i = batchSize - 1; i >= 0; i--)
            {
                Assert.That((long)_db.ListMove(queueWorking, queue, ListSide.Left, ListSide.Right), Is.EqualTo(i));
            }
        }
        catch (RedisServerException ex)
        {
            Console.WriteLine(ex.Message);

            for (int i = 0; i < batchSize; i++)
            {
                Assert.That((long)_db.ListRightPopLeftPush(queue, queueWorking), Is.EqualTo(i));
            }

            //Roolback

            for (int i = 0; i < batchSize; i++)
            {
                Assert.That((long)_db.ListRightPopLeftPush(queueWorking, queue), Is.EqualTo(i));
            }
        }

        

        //Assert.That(_db.ListRightPopLeftPush("q1", "q1-working"), Is.EqualTo(0));
    }


}
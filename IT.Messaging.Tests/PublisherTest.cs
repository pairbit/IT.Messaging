namespace IT.Messaging.Tests;

public class PublisherTest
{
    private IChannel _channel;

    [SetUp]
    public void Setup(IChannel channel)
    {
        _channel = channel;
    }

    [Test]
    public void TestGuid()
    {
        var guid = Guid.NewGuid();

        _channel.Subscribe((Guid guid, string? channel) =>
        {
            Console.WriteLine($"[{channel}] -> {guid}");
        }, "POIB-*");

        _channel.Publish(guid, "POIB-hash");

        Assert.Pass();
    }

    [Test]
    public void TestGuidMulti()
    {
        _channel.Subscribe((Guid[] guid, string? channel) =>
        {
            foreach (var g in guid)
            {
                Console.WriteLine($"[{channel}] -> {g}");
            }

        }, "POIB-*");

        _channel.Publish(new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, "POIB-hash");

        Assert.Pass();
    }

    [Test]
    public void SigningTest()
    {
        _channel.Subscribe<Guid>(cleanRequest, "*-cleanRequest");
        _channel.Subscribe<Guid>(verifyTransformSignature, "*-verifyTransformSignature");
        _channel.Subscribe<Guid>(enhanceSign, "*-enhanceSign");

        _channel.Publish(new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, "POIB-enhanceSign");
        _channel.Publish(new[] { Guid.NewGuid() }, "UCFK-enhanceSign");

        Assert.Pass();
    }

    private void verifyTransformSignature(Guid[] guid, string? channel) { }

    private void enhanceSign(Guid[] guid, string? channel) { }

    private void cleanRequest(Guid[] guid, string? channel) { }
}
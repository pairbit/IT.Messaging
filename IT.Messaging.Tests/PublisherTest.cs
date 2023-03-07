namespace IT.Messaging.Tests;

public class PublisherTest
{
    private IChannel _channel;

    [SetUp]
    public void Setup(IChannel channel)
    {
        _channel = channel;
    }

    /// <summary>
    /// Для простых систем подойдет такой режим, 
    /// подписываемся на обработку одной очереди по умолчанию.
    /// Недостаток состоит в том, если одна система создает 50% и более нагрузки, другие системы могут страдать.
    /// </summary>
    [Test]
    public void Subscribe_Default()
    {
        _channel.Subscribe<Guid>(processDefault);

        Publish(_channel);

        Assert.Pass();
    }

    /// <summary>
    /// Для разграничения, можно создать очередь под самую нагруженную систему отдельно
    /// или под каждую систему
    /// </summary>
    [Test]
    public void Subscribe_BySystem()
    {
        //_channel.Bind("eb.*", "eb");

        _channel.Subscribe<Guid>(processDefault);
        _channel.Subscribe<Guid>(processDefault, key: "eb");

        Publish(_channel);

        Assert.Pass();
    }

    /// <summary>
    /// Очередь по операциям
    /// </summary>
    [Test]
    public void Subscribe_ByOperation()
    {
        //_channel.Bind("*.enhance.*", "enhance");
        //_channel.Bind("*.prepare.*", "prepare");
        //_channel.Bind("*.generate-view.*", "generate-view");

        _channel.Subscribe<Guid>(processDefault);
        _channel.Subscribe<Guid>(enhanceSign, key: "enhance");
        _channel.Subscribe<Guid>(prepareSign, key: "prepare");
        _channel.Subscribe<Guid>(generateView, key: "generate-view");

        Publish(_channel);

        Assert.Pass();
    }

    [Test]
    public void Subscribe_Optimize()
    {
        //при публикации "eb.generate-view-html.big" первое правило подходит и указан параметр остановить дальнейшую обработку
        //идея данной очереди в том, что очень долгие запросы обрабатываются в отдельных потоках и не засоряют общие очереди
        //_channel.Bind("*.big", "big", stop: true);

        //_channel.Bind("eb.*", "eb");

        _channel.Subscribe<Guid>(processDefault);
        _channel.Subscribe<Guid>(processDefault, key: "eb");
        _channel.Subscribe<Guid>(processDefault, key: "big");

        Publish(_channel);

        Assert.Pass();
    }

    private void Publish(IPublisher publisher)
    {
        publisher.Publish(new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, "eb.enhance.normal");
        publisher.Publish(new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, "eb.enhance.small");
        publisher.Publish(new[] { Guid.NewGuid() }, "eb.generate-view-html.big");
        publisher.Publish(new[] { Guid.NewGuid() }, "eb.generate-view-pdf.big");

        publisher.Publish(new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, "ucfk.prepare.small");

        publisher.Publish(new[] { Guid.NewGuid() }, "gmu.prepare.normal");
        publisher.Publish(new[] { Guid.NewGuid() }, "gmu.generate-view-html.big");
        publisher.Publish(new[] { Guid.NewGuid() }, "gmu.generate-view-pdf.big");
    }

    private void processDefault(Guid guid, string? queue, CancellationToken token) 
    {
        var arr = guid.ToByteArray();

        var type = arr[^1];

        if (type == 1) prepareSign(guid, queue, token);
        if (type == 2) enhanceSign(guid, queue, token);
        if (type == 3) generateView(guid, queue, token);
    }

    private void enhanceSign(Guid guid, string? queue, CancellationToken token) { }

    private void prepareSign(Guid guid, string? queue, CancellationToken token) { }

    private void generateView(Guid guid, string? queue, CancellationToken token) { }
}
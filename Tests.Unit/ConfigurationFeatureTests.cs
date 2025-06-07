using MsSentinel.BanffProtect.Application;
using MsSentinel.BanffProtect.Entities;
using MsSentinel.BanffProtect.Tests.Unit.Fakes;

public class ConfigurationFeatureTests
{
    private ConfigurationFeature? feature;

    [SetUp]
    public void SetUp()
    {
        feature = new ConfigurationFeature(new FakeDistributedCache());
    }

    [Test]
    public void Empty()
    {
        Assert.That(feature, Is.Not.Null);
    }

    [Test]
    public async Task NotExists()
    {
        var exists = await feature!.IfExistsAsync();

        Assert.That(exists,Is.False);
    }

    [Test]
    public async Task Exists()
    {
        var config = new ConnectorConfiguration();
        await feature!.StoreAsync(config);

        var exists = await feature!.IfExistsAsync();

        Assert.That(exists,Is.True);
    }

    [Test]
    public async Task LoadAndSave()
    {
        var expected = new ConnectorConfiguration()
        {
            TenantId = Guid.NewGuid(),
            ApplicationID = Guid.NewGuid(),
            ApplicationSecret = "Secret",
            CollectionEndpoint = new Uri("https://azurecontainerapps.io/"),
            RuleId = "RuleId"
        };
        await feature!.StoreAsync(expected);

        var actual = await feature!.LoadAsync();

        Assert.That(actual,Is.EqualTo(expected));
    }
}

using MsSentinel.BanffProtect.Application;
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
}

using MsSentinel.BanffProtect.Entities;

namespace MsSentinel.BanffProtect.Application;

/// <summary>
/// Provides persistent storage for a single connector configurations
/// </summary>
public class ConfigurationFeature
{
    /// <summary>
    /// Load the stored connector configuration
    /// </summary>
    /// <returns>Connector configuration</returns>
    public Task<ConnectorConfiguration> LoadAsync()
    {
        return Task.FromResult(new ConnectorConfiguration());
    }

    /// <summary>
    /// Store the stored connector configuration
    /// </summary>
    /// <returns>Connector configuration</returns>
    public Task StoreAsync(ConnectorConfiguration _)
    {
        return Task.CompletedTask;
    }
}

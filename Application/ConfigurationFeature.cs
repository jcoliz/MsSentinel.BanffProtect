using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using MsSentinel.BanffProtect.Entities;

namespace MsSentinel.BanffProtect.Application;

/// <summary>
/// Provides persistent storage for a single connector configurations
/// Backed by a distributed cache
/// </summary>
public class ConfigurationFeature(IDistributedCache distributedCache)
{
    /// <summary>
    /// Returns true if there is a configuration available to be read
    /// </summary>
    /// <returns>True if there is a configuration available </returns>
    public async Task<bool> IfExistsAsync()
    {
        var result = false;
        try
        {
            var _ = await GetValuesAsync();
            result = true;
        }
        catch (KeyNotFoundException)
        {
            // No action, result remains false
        }

        return result;
    }

    /// <summary>
    /// Load the stored connector configuration
    /// </summary>
    /// <returns>Connector configuration</returns>
    public async Task<ConnectorConfiguration> LoadAsync()
    {
        // Get values from store
        var values = await GetValuesAsync();

        // place into a result
        return new ConnectorConfiguration()
        {
            TenantId = Guid.Parse(values["TenantId"]),
            ApplicationID = Guid.Parse(values["ApplicationID"]),
            ApplicationSecret = values["ApplicationSecret"],
            CollectionEndpoint = new Uri(values["CollectionEndpoint"]),
            RuleId = values["RuleId"]
        };
    }

    private async Task<Dictionary<string,string>> GetValuesAsync()
    {
        var values = new Dictionary<string,string>();
        foreach(var field in _fields)
        {
            values[field] = 
                (await distributedCache.GetStringAsync($"{_prefix}.{field}")) 
                ?? throw new KeyNotFoundException($"No value found for key ${field}");
        }

        return values;
    }

    private readonly string _prefix = "ConnectorConfiguration";
    private readonly string[] _fields = [ "TenantId", "ApplicationID", "ApplicationSecret", "CollectionEndpoint", "RuleId" ];

    /// <summary>
    /// Store the stored connector configuration
    /// </summary>
    /// <param name="config">Connector configuration to store</param>
    public async Task StoreAsync(ConnectorConfiguration config)
    {
        byte[] bytesOf(object o) => Encoding.UTF8.GetBytes(o.ToString()!);

        await distributedCache.SetAsync($"{_prefix}.TenantId",bytesOf(config.TenantId));
        await distributedCache.SetAsync($"{_prefix}.ApplicationID",bytesOf(config.ApplicationID));
        await distributedCache.SetAsync($"{_prefix}.ApplicationSecret",bytesOf(config.ApplicationSecret));
        await distributedCache.SetAsync($"{_prefix}.CollectionEndpoint",bytesOf(config.CollectionEndpoint));
        await distributedCache.SetAsync($"{_prefix}.RuleId",bytesOf(config.RuleId));
    }
}

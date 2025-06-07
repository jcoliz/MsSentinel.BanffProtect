namespace MsSentinel.BanffProtect.Entities;

/// <summary>
/// Contains parameters needed for a connector configuration to Microsoft Sentinel
/// </summary>
public record ConnectorConfiguration
{
    public Guid TenantId { get; init; } = Guid.Empty;
    public Guid ApplicationID { get; init; } = Guid.Empty;
    public string ApplicationSecret { get; init; } = string.Empty;
    public Uri CollectionEndpoint{ get; init; } = new Uri("http://localhost/");
    public string RuleId { get; init; } = string.Empty;
}
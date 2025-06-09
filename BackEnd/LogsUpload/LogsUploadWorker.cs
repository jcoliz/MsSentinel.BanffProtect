using System.Runtime.CompilerServices;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using MsSentinel.BanffProtect.Application;
using MsSentinel.BanffProtect.Backend.Logs.Shape;
using MsSentinel.BanffProtect.Backend.Metrics;

namespace MsSentinel.BanffProtect.Backend.LogsUpload;

public partial class LogsUploadWorker(
    ConfigurationFeature configFeature,
    ILogger<LogsUploadWorker> logger,
    BackEndMetrics metrics
) : BackgroundService
{
    private LogsIngestionClient? logsClient;

    private string? ruleId;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (await CheckLogsClientAsync())
                    {
                        var telemetry = GenerateTelemetryV2();
                        await UploadToLogsAsync("Custom-jamfprotecttelemetryv2", telemetry, stoppingToken);

                        var logs = GenerateLogs();
                        await UploadToLogsAsync("Custom-jamfprotectunifiedlogs",logs!,stoppingToken);

                        var alerts = GenerateAlerts();
                        await UploadToLogsAsync("Custom-jamfprotectalerts",alerts,stoppingToken);

                        logOk();
                    }
                }
                catch( Exception ex )
                {
                    logFail(ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }            
        }
        catch (TaskCanceledException)
        {
            // Normal exit
        }
        catch (Exception ex)
        {
            logCritical(ex);
        }
    }

    private async Task<bool> CheckLogsClientAsync()
    {
        try
        {
            if (logsClient is not null)
            {
                return true;
            }

            if (! await configFeature.IfExistsAsync())
            {
                logNoConfig();
                return false;
            }

            // At this point, we don't already have a client, but we DO have a config.
            // So we can create our own client now.

            var config = await configFeature.LoadAsync();
            logConfigFound(config.CollectionEndpoint);

            var token = new ClientSecretCredential(config.TenantId.ToString(), config.ApplicationID.ToString(), config.ApplicationSecret);
            logsClient = new LogsIngestionClient(config.CollectionEndpoint,token);
            ruleId = config.RuleId;

            return true;
        }
        catch( Exception ex )
        {
            logFail(ex);

            return false;
        }
    }

    protected ICollection<CustomJamfprotecttelemetryv2> GenerateTelemetryV2()
    {
        var result = new List<CustomJamfprotecttelemetryv2>()
        {
            new()
            {
                Metadata = new{ product = "product", schemaversion = "schemaversion" },
                Host = new{ hostname = "hostname" }
            }
        };

        return result;
    }

    protected ICollection<CustomJamfprotectunifiedlogs> GenerateLogs()
    {
        var result = new List<CustomJamfprotectunifiedlogs>()
        {
            new()
            {
                Input = new{ 
                    host = new{ protectVersion = "protectVersion" },
                    match = new{ severity = 1, uuid = Guid.NewGuid().ToString() }
                }
            }
        };

        return result;
    }

    protected ICollection<CustomJamfprotectalerts> GenerateAlerts()
    {
        var result = new List<CustomJamfprotectalerts>()
        {
            new()
            {
                Input = new{ 
                    host = new{ protectVersion = "protectVersion" },
                    match = new{ severity = 1, uuid = Guid.NewGuid().ToString() },
                    eventType = "GPDownloadEvent"
                }
            }
        };

        return result;
    }


    public async Task UploadToLogsAsync<T>(string streamName, ICollection<T> dataPoints, CancellationToken stoppingToken)
    {
        try
        {
            var response = await logsClient!.UploadAsync
            (
                ruleId: ruleId,
                streamName: streamName,
                logs: dataPoints,
                cancellationToken: stoppingToken
            )
            .ConfigureAwait(false);

            metrics.LogsSent(1, streamName);
            metrics.LogLinesSent(dataPoints.Count, streamName);

            switch (response?.IsError)
            {
                case null:
                    logSendNoResponse();
                    break;

                case true:
                    logSendFail(response.Status);
                    break;

                default:
                    logSentOk(streamName, response.Status);
                    break;
            }
        }
        catch (Exception ex)
        {
            logFail(ex);
        }
    }    

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK", EventId = 1000)]
    public partial void logOk([CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: No configuration available, skipping", EventId = 1001)]
    public partial void logNoConfig([CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: Found configuration. Creating connection to {Endpoint}", EventId = 1002)]
    public partial void logConfigFound(Uri endpoint,[CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Location}: Failed", EventId = 1008)]
    public partial void logFail(Exception ex,[CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Critical, Message = "{Location}: Critical failure", EventId = 1009)]
    public partial void logCritical(Exception ex,[CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: Sent OK {Status} to {Stream}", EventId = 1100)]
    public partial void logSentOk(string Stream, int Status, [CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Location}: Send failed, returned no response", EventId = 1107)]
    public partial void logSendNoResponse([CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Location}: Send failed {Status}", EventId = 1108)]
    public partial void logSendFail(int Status, [CallerMemberName] string? location = null);    
}
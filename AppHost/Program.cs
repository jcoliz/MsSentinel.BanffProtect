var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MsSentinel_BanffProtect_BackEnd>("backend")
    .WithEnvironment("Logging__Console__FormatterName","systemd");

builder.AddProject<Projects.MsSentinel_BanffProtect_FrontEnd_Blazor>("frontend-blazor")
    .WithEnvironment("Logging__Console__FormatterName","systemd")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

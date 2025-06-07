var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MsSentinel_BanffProtect_ApiService>("apiservice");

builder.AddProject<Projects.MsSentinel_BanffProtect_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

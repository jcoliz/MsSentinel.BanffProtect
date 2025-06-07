var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MsSentinel_BanffProtect_BackEnd>("apiservice");

builder.AddProject<Projects.MsSentinel_BanffProtect_FrontEnd_Blazor>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MsSentinel_BanffProtect_BackEnd>("backend");

builder.AddProject<Projects.MsSentinel_BanffProtect_FrontEnd_Blazor>("frontend-blazor")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

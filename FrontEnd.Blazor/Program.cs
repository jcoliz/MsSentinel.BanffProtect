using MsSentinel.BanffProtect.BackEnd.Api;
using MsSentinel.BanffProtect.FrontEnd.Blazor.Components;
using MsSentinel.BanffProtect.FrontEnd.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddTomlFile("config.toml", optional: true, reloadOnChange: true);


// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents(options => 
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<ApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://backend");
    });

// Load options from configuration. These all us to provide defaults for testing
builder.Services.Configure<LogIngestionOptions>(
    builder.Configuration.GetSection(LogIngestionOptions.Section)
);
builder.Services.Configure<IdentityOptions>(
    builder.Configuration.GetSection(IdentityOptions.Section)
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();

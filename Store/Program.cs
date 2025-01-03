using Store.Components;
using Store.Services;
//using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductService>();

//.AddStandardResilienceHandler() adds resiliency to the http client !!!
builder.Services.AddHttpClient<ProductService>(c =>
{
    var url = builder.Configuration["ProductEndpoint"] ?? throw new InvalidOperationException("ProductEndpoint is not set");

    c.BaseAddress = new(url);
});    
// }).AddStandardResilienceHandler(options =>
// {
//     //default timeout was 30s, but we changed it to 260s
//     options.Retry.MaxRetryAttempts = 7;
//     options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
//     {
//         Timeout = TimeSpan.FromMinutes(5)
//     };
// });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add observability code here
builder.Services.AddObservability("Store", builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapObservability();

app.Run();

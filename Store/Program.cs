using Store.Components;
using Store.Services;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Flagsmith;

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

// Add redaction code here
//builder.Services.AddRedaction();
builder.Services.AddRedaction(configure =>
{
    configure.SetRedactor<ErasingRedactor>(new DataClassificationSet(DataClassifications.EUPDataClassification));
    configure.SetRedactor<EShopCustomRedactor>(new DataClassificationSet(DataClassifications.EUIIDataClassification));
});

builder.Services.AddLogging(logging =>
{    
    logging.EnableRedaction();
    logging.AddJsonConsole(); //Enable structure logs on the console to view the redacted data.
});

var apiKey = builder.Configuration["Flagsmith:EnvironmentKey"]; 
var apiUrl = builder.Configuration["Flagsmith:ApiUrl"]; 
builder.Services.AddSingleton(new FlagsmithProxy(apiKey, apiUrl));

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

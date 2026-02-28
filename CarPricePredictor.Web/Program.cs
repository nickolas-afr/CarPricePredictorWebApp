#pragma warning disable SKEXP0070

using CarPricePredictor.Web.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Load secrets configuration (for API keys - excluded from source control)
builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IPredictionService, MLPredictionService>();
builder.Services.AddSingleton<ICarDataService, CarDataService>();
builder.Services.AddScoped<IVinDecoderService, VinDecoderService>();
builder.Services.AddScoped<IDealScoreService, DealScoreService>();

// Register recommendation service
builder.Services.AddScoped<ICarRecommendationService, CarRecommendationService>();

// Register Semantic Kernel with Ollama (only if enabled)
var ollamaEnabled = builder.Configuration.GetValue<bool>("Ollama:Enabled");
var ollamaBaseUrl = builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
var ollamaModel = builder.Configuration["Ollama:Model"] ?? "llama3.2:3b";

if (ollamaEnabled)
{
    builder.Services.AddSingleton<Kernel>(sp =>
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOllamaChatCompletion(
            modelId: ollamaModel,
            endpoint: new Uri(ollamaBaseUrl)
        );
        return kernelBuilder.Build();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

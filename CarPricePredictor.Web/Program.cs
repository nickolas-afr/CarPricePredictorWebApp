using CarPricePredictor.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IPredictionService, MLPredictionService>();
builder.Services.AddSingleton<ICarDataService, CarDataService>();
builder.Services.AddScoped<IMarketComparisonService, MarketComparisonService>();
builder.Services.AddScoped<IVinDecoderService, VinDecoderService>();
builder.Services.AddScoped<IDealScoreService, DealScoreService>();

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

using CarPricePredictor.Web.Models;
using System.Text.Json;

namespace CarPricePredictor.Web.Services;

public class MarketComparisonService : IMarketComparisonService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MarketComparisonService> _logger;

    public MarketComparisonService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MarketComparisonService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<MarketComparisonResult> GetMarketComparisonAsync(CarInputModel input)
    {
        // Note: Real car listing APIs (AutoScout24, Mobile.de, etc.) are paid or restricted.
        // This implementation uses enhanced mock data to simulate market comparison.
        // The mock data is generated based on the input car's characteristics.
        
        _logger.LogInformation("Generating market comparison data for {Make} {Model} {Year}", 
            input.Make, input.Model, input.Year);

        await Task.Delay(100); // Simulate API call delay for better UX

        return GetMockMarketData(input);
    }

    private MarketComparisonResult GetMockMarketData(CarInputModel input)
    {
        // Generate mock data for demonstration purposes
        var random = new Random();
        var basePrice = input.Price > 0 ? input.Price : 25000;
        
        var listings = new List<MarketListing>();
        for (int i = 0; i < 5; i++)
        {
            var priceVariation = (float)(random.NextDouble() * 0.2 - 0.1); // ±10%
            listings.Add(new MarketListing
            {
                Source = new[] { "AutoScout24", "Mobile.de", "Cars.com", "AutoTrader" }[random.Next(4)],
                Make = input.Make,
                Model = input.Model,
                Year = input.Year + random.Next(-2, 3),
                Mileage = input.Mileage + random.Next(-10000, 10000),
                Price = basePrice * (1 + priceVariation),
                Url = $"https://example.com/listing-{i + 1}"
            });
        }

        var result = new MarketComparisonResult
        {
            Listings = listings,
            AverageMarketPrice = listings.Average(l => l.Price),
            HasData = true
        };

        if (input.Price > 0)
        {
            result.PriceComparisonPercentage = 
                ((input.Price - result.AverageMarketPrice) / result.AverageMarketPrice) * 100;
        }

        return result;
    }
}

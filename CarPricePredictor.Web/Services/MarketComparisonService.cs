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
        var result = new MarketComparisonResult();

        try
        {
            var apiKey = _configuration["Carapis:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey) || apiKey == "your-carapis-api-key-here")
            {
                _logger.LogWarning("Carapis API key not configured. Using mock data.");
                return GetMockMarketData(input);
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            // Carapis API endpoint for searching listings
            var baseUrl = _configuration["Carapis:BaseUrl"] ?? "https://api.carapis.com/v1";
            var yearMin = input.Year - 2;
            var yearMax = input.Year + 2;
            var mileageMin = Math.Max(0, input.Mileage - 15000);
            var mileageMax = input.Mileage + 15000;

            var url = $"{baseUrl}/listings/search?make={Uri.EscapeDataString(input.Make)}&model={Uri.EscapeDataString(input.Model)}&year_min={yearMin}&year_max={yearMax}&mileage_min={mileageMin}&mileage_max={mileageMax}&limit=5";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var listings = JsonSerializer.Deserialize<List<MarketListing>>(content, jsonOptions);

                if (listings != null && listings.Any())
                {
                    result.Listings = listings.Take(5).ToList();
                    result.AverageMarketPrice = result.Listings.Average(l => l.Price);
                    result.HasData = true;

                    if (input.Price > 0)
                    {
                        result.PriceComparisonPercentage = 
                            ((input.Price - result.AverageMarketPrice) / result.AverageMarketPrice) * 100;
                    }
                }
                else
                {
                    result.ErrorMessage = "No similar listings found in the market.";
                }
            }
            else
            {
                _logger.LogError("Carapis API returned error: {StatusCode}", response.StatusCode);
                result.ErrorMessage = "Unable to fetch market data at this time.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching market comparison data");
            result.ErrorMessage = "An error occurred while fetching market data.";
        }

        return result;
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

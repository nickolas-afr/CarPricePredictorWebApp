using CarPricePredictor.Web.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CarPricePredictor.Web.Services;

public class VinDecoderService : IVinDecoderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VinDecoderService> _logger;

    public VinDecoderService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<VinDecoderService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsValidVin(string vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
            return false;

        // VIN must be exactly 17 characters
        if (vin.Length != 17)
            return false;

        // VIN cannot contain I, O, or Q
        if (Regex.IsMatch(vin, "[IOQ]", RegexOptions.IgnoreCase))
            return false;

        // VIN must be alphanumeric
        if (!Regex.IsMatch(vin, "^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.IgnoreCase))
            return false;

        return true;
    }

    public async Task<VinDecodeResult> DecodeVinAsync(string vin)
    {
        var result = new VinDecodeResult();

        if (!IsValidVin(vin))
        {
            result.ErrorMessage = "Invalid VIN format. VIN must be 17 characters and cannot contain I, O, or Q.";
            return result;
        }

        try
        {
            var apiKey = _configuration["Carapis:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey) || apiKey == "your-carapis-api-key-here")
            {
                _logger.LogWarning("Carapis API key not configured. Using mock VIN decoder.");
                return GetMockVinData(vin);
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            var baseUrl = _configuration["Carapis:BaseUrl"] ?? "https://api.carapis.com/v1";
            var url = $"{baseUrl}/vin/decode/{vin}";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var vinData = JsonSerializer.Deserialize<VinDecodeResult>(content, jsonOptions);

                if (vinData != null)
                {
                    vinData.Success = true;
                    return vinData;
                }
                else
                {
                    result.ErrorMessage = "Unable to decode VIN.";
                }
            }
            else
            {
                _logger.LogError("Carapis VIN decode API returned error: {StatusCode}", response.StatusCode);
                result.ErrorMessage = "Unable to decode VIN at this time. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decoding VIN: {Vin}", vin);
            result.ErrorMessage = "An error occurred while decoding the VIN.";
        }

        return result;
    }

    private VinDecodeResult GetMockVinData(string vin)
    {
        // Generate mock data for demonstration purposes
        return new VinDecodeResult
        {
            Success = true,
            Make = "BMW",
            Model = "3 Series",
            Year = 2020,
            Trim = "330i",
            Engine = "2.0L Turbo I4",
            Transmission = "Automatic",
            FuelType = "Petrol",
            BodyType = "Sedan"
        };
    }
}

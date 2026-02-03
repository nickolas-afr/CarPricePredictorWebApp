using CarPricePredictor.Web.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            // Check if user wants to use mock data
            var useMockData = _configuration.GetValue<bool>("VinDecoder:UseMockData");
            if (useMockData)
            {
                _logger.LogInformation("Using mock VIN decoder as configured.");
                return GetMockVinData(vin);
            }

            // Use RapidAPI VIN Decoder Europe2 (for European cars)
            var apiKey = _configuration["VinDecoder:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey) || apiKey == "API_KEY_PLACEHOLDER")
            {
                _logger.LogWarning("RapidAPI key not configured. Using mock VIN decoder.");
                return GetMockVinData(vin);
            }

            var httpClient = _httpClientFactory.CreateClient();
            var apiHost = _configuration["VinDecoder:ApiHost"] ?? "vin-decoder-europe2.p.rapidapi.com";
            var baseUrl = _configuration["VinDecoder:BaseUrl"] ?? "https://vin-decoder-europe2.p.rapidapi.com";
            var url = $"{baseUrl}/vin_decoder?vin={vin}";

            // Add RapidAPI headers
            httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
            httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", apiHost);

            _logger.LogInformation("Decoding VIN using RapidAPI VIN Decoder Europe2: {Vin}", vin);
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var rapidApiResponse = JsonSerializer.Deserialize<RapidApiVinResponse>(content, jsonOptions);

                if (rapidApiResponse != null && rapidApiResponse.Code == 200 && !string.IsNullOrEmpty(rapidApiResponse.Make))
                {
                    result = MapRapidApiToVinDecodeResult(rapidApiResponse);
                    result.Success = true;
                    _logger.LogInformation("Successfully decoded VIN: {Make} {Model} {Year}", result.Make, result.Model, result.Year);
                    return result;
                }
                else
                {
                    result.ErrorMessage = "Unable to decode VIN. No data returned from API.";
                }
            }
            else
            {
                _logger.LogError("VIN decode API returned error: {StatusCode}", response.StatusCode);
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

    private VinDecodeResult MapRapidApiToVinDecodeResult(RapidApiVinResponse response)
    {
        var result = new VinDecodeResult
        {
            Make = response.Make ?? "",
            Model = response.Model ?? "",
            Trim = response.BodyStyle ?? "", // Use body_style as trim
            Engine = response.EngineType ?? "", // Use engine_type for engine info
            Transmission = response.Driveline ?? "", // Use driveline (FWD/RWD/AWD) as best available
            BodyType = response.BodyType ?? ""
        };

        // Parse year from model_year
        if (int.TryParse(response.ModelYear, out int year))
        {
            result.Year = year;
        }

        // Map fuel type
        result.FuelType = MapFuelType(response.FuelType ?? "");

        return result;
    }

    private string MapFuelType(string fuelType)
    {
        if (string.IsNullOrEmpty(fuelType))
            return "";

        var lowerFuelType = fuelType.ToLower();
        
        if (lowerFuelType.Contains("gasoline") || lowerFuelType.Contains("petrol") || lowerFuelType.Contains("benzin"))
            return "Petrol";
        if (lowerFuelType.Contains("diesel"))
            return "Diesel";
        if (lowerFuelType.Contains("electric") || lowerFuelType.Contains("elektro"))
            return "Electric";
        if (lowerFuelType.Contains("hybrid"))
            return "Hybrid";
        if (lowerFuelType.Contains("compressed natural gas") || lowerFuelType.Contains("cng") || lowerFuelType.Contains("erdgas"))
            return "CNG";
        
        return fuelType;
    }

    // RapidAPI VIN Decoder Europe2 response model
    private class RapidApiVinResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        
        [JsonPropertyName("make")]
        public string? Make { get; set; }
        
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        
        [JsonPropertyName("model_year")]
        public string? ModelYear { get; set; }
        
        [JsonPropertyName("body_style")]
        public string? BodyStyle { get; set; }
        
        [JsonPropertyName("engine_type")]
        public string? EngineType { get; set; }
        
        [JsonPropertyName("fuel_type")]
        public string? FuelType { get; set; }
        
        [JsonPropertyName("body_type")]
        public string? BodyType { get; set; }
        
        [JsonPropertyName("engine_horsepower")]
        public string? EngineHorsepower { get; set; }
        
        [JsonPropertyName("driveline")]
        public string? Driveline { get; set; }
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

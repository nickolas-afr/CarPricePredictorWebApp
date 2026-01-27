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
            // Check if user wants to use mock data
            var useMockData = _configuration.GetValue<bool>("VinDecoder:UseMockData");
            if (useMockData)
            {
                _logger.LogInformation("Using mock VIN decoder as configured.");
                return GetMockVinData(vin);
            }

            // Use NHTSA vPIC API (free, no API key required)
            var httpClient = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["VinDecoder:BaseUrl"] ?? "https://vpic.nhtsa.dot.gov/api/vehicles";
            var url = $"{baseUrl}/DecodeVin/{vin}?format=json";

            _logger.LogInformation("Decoding VIN using NHTSA vPIC API: {Vin}", vin);
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var nhtsaResponse = JsonSerializer.Deserialize<NhtsaVinResponse>(content, jsonOptions);

                if (nhtsaResponse?.Results != null && nhtsaResponse.Results.Count > 0)
                {
                    result = MapNhtsaToVinDecodeResult(nhtsaResponse.Results);
                    result.Success = true;
                    _logger.LogInformation("Successfully decoded VIN: {Make} {Model} {Year}", result.Make, result.Model, result.Year);
                    return result;
                }
                else
                {
                    result.ErrorMessage = "Unable to decode VIN. No data returned from NHTSA.";
                }
            }
            else
            {
                _logger.LogError("NHTSA VIN decode API returned error: {StatusCode}", response.StatusCode);
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

    private VinDecodeResult MapNhtsaToVinDecodeResult(List<NhtsaVariable> variables)
    {
        var result = new VinDecodeResult();

        foreach (var variable in variables)
        {
            if (string.IsNullOrEmpty(variable.Value))
                continue;

            switch (variable.Variable?.ToLower())
            {
                case "make":
                    result.Make = variable.Value;
                    break;
                case "model":
                    result.Model = variable.Value;
                    break;
                case "model year":
                    if (int.TryParse(variable.Value, out int year))
                        result.Year = year;
                    break;
                case "trim":
                    result.Trim = variable.Value;
                    break;
                case "engine number of cylinders":
                    result.Engine = variable.Value;
                    break;
                case "transmission style":
                    result.Transmission = variable.Value;
                    break;
                case "fuel type - primary":
                    result.FuelType = MapFuelType(variable.Value);
                    break;
                case "body class":
                    result.BodyType = variable.Value;
                    break;
            }
        }

        return result;
    }

    private string MapFuelType(string nhtsaFuelType)
    {
        if (string.IsNullOrEmpty(nhtsaFuelType))
            return "";

        var lowerFuelType = nhtsaFuelType.ToLower();
        
        if (lowerFuelType.Contains("gasoline"))
            return "Petrol";
        if (lowerFuelType.Contains("diesel"))
            return "Diesel";
        if (lowerFuelType.Contains("electric"))
            return "Electric";
        if (lowerFuelType.Contains("hybrid"))
            return "Hybrid";
        if (lowerFuelType.Contains("compressed natural gas") || lowerFuelType.Contains("cng"))
            return "CNG";
        
        return nhtsaFuelType;
    }

    // NHTSA API response models
    private class NhtsaVinResponse
    {
        public int Count { get; set; }
        public string? Message { get; set; }
        public List<NhtsaVariable> Results { get; set; } = new();
    }

    private class NhtsaVariable
    {
        public string? Variable { get; set; }
        public string? Value { get; set; }
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

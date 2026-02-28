#pragma warning disable SKEXP0070

using System.Globalization;
using System.Text;
using System.Text.Json;
using CarPricePredictor.Web.Data;
using CarPricePredictor.Web.Models;
using Microsoft.SemanticKernel;

namespace CarPricePredictor.Web.Services;

public class CarRecommendationService : ICarRecommendationService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IPredictionService _predictionService;
    private readonly IDealScoreService _dealScoreService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CarRecommendationService> _logger;
    private readonly Kernel? _kernel;

    public CarRecommendationService(
        IWebHostEnvironment environment,
        IPredictionService predictionService,
        IDealScoreService dealScoreService,
        IConfiguration configuration,
        ILogger<CarRecommendationService> logger,
        Kernel? kernel = null)
    {
        _environment = environment;
        _predictionService = predictionService;
        _dealScoreService = dealScoreService;
        _configuration = configuration;
        _logger = logger;
        _kernel = kernel;
    }

    public async Task<List<CarRecommendationResult>> GetRecommendationsAsync(CarRecommendationFilter filter)
    {
        // Step 1: Read and filter CSV
        var allCars = ReadCsvData();
        var filtered = ApplyFilters(allCars, filter);

        if (!filtered.Any())
        {
            _logger.LogWarning("No cars matched the filter criteria");
            return new List<CarRecommendationResult>();
        }

        // Step 2: Group and rank candidates
        var grouped = filtered
            .GroupBy(c => new { c.Make, c.Model })
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        // Step 3: Run ML predictions
        var candidates = new List<CandidateInfo>();
        foreach (var group in grouped)
        {
            var rows = group.ToList();
            var medianYear = Median(rows.Select(r => r.Year));
            var medianMileage = Median(rows.Select(r => r.Mileage));
            var medianHp = Median(rows.Select(r => r.Hp));
            var medianPrice = Median(rows.Select(r => r.Price));

            var input = new CarInputModel
            {
                Make = group.Key.Make,
                Model = group.Key.Model,
                Year = (int)medianYear,
                Mileage = medianMileage,
                Hp = medianHp,
                Fuel = rows.First().Fuel,
                Gear = rows.First().Gear,
                OfferType = rows.First().OfferType == "New" ? 1 : 0,
                Price = medianPrice
            };

            var prediction = _predictionService.PredictPrice(input);
            var dealScore = _dealScoreService.CalculateDealScore(prediction.PredictedPrice, medianPrice);

            var metadata = CarModelMetadata.Get(group.Key.Make, group.Key.Model);

            candidates.Add(new CandidateInfo
            {
                Make = group.Key.Make,
                Model = group.Key.Model,
                MedianPrice = medianPrice,
                PredictedPrice = prediction.PredictedPrice,
                MinPrice = prediction.MinPrice,
                MaxPrice = prediction.MaxPrice,
                DealScore = dealScore,
                BodyType = metadata?.BodyType ?? "Unknown",
                Seats = metadata?.TypicalSeats ?? 5,
                MinHp = rows.Min(r => r.Hp),
                MaxHp = rows.Max(r => r.Hp),
                Count = rows.Count
            });
        }

        // Step 4: Try LLM
        var ollamaEnabled = _configuration.GetValue<bool>("Ollama:Enabled");
        if (ollamaEnabled && _kernel != null)
        {
            try
            {
                var prompt = BuildPrompt(filter, candidates);
                var result = await _kernel.InvokePromptAsync(prompt);
                var json = result.ToString();

                var llmResults = ParseLlmResponse(json, candidates);
                if (llmResults.Count > 0)
                {
                    return llmResults;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "LLM call failed, falling back to filtered results");
            }
        }

        // Step 5: Fallback
        return candidates
            .Take(5)
            .Select(c => new CarRecommendationResult
            {
                Make = c.Make,
                Model = c.Model,
                EstimatedPrice = c.PredictedPrice,
                MinPrice = c.MinPrice,
                MaxPrice = c.MaxPrice,
                DealScore = c.DealScore,
                LlmExplanation = "This model matches your criteria based on price, fuel type, and power output."
            })
            .ToList();
    }

    private List<CarRow> ReadCsvData()
    {
        var datasetPath = Path.Combine(_environment.ContentRootPath, "..", "CarPricePredictor.ML", "Data", "autoscout24-germany-dataset.csv");

        if (File.Exists(datasetPath))
        {
            try
            {
                return ParseCsv(datasetPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read CSV, using fallback data");
            }
        }
        else
        {
            _logger.LogWarning("Dataset not found at {Path}, using fallback data", datasetPath);
        }

        return GetFallbackData();
    }

    private List<CarRow> ParseCsv(string path)
    {
        var cars = new List<CarRow>();

        using var reader = new StreamReader(path);
        reader.ReadLine(); // skip header

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = ParseCsvLine(line);
            if (values.Length < 9) continue;

            if (float.TryParse(values[0], CultureInfo.InvariantCulture, out var mileage) &&
                float.TryParse(values[6], CultureInfo.InvariantCulture, out var price) &&
                float.TryParse(values[7], CultureInfo.InvariantCulture, out var hp) &&
                float.TryParse(values[8], CultureInfo.InvariantCulture, out var year))
            {
                cars.Add(new CarRow
                {
                    Mileage = mileage,
                    Make = values[1].Trim(),
                    Model = values[2].Trim(),
                    Fuel = values[3].Trim(),
                    Gear = values[4].Trim(),
                    OfferType = values[5].Trim(),
                    Price = price,
                    Hp = hp,
                    Year = year
                });
            }
        }

        return cars;
    }

    private string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
                inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
                current.Append(c);
        }
        values.Add(current.ToString());
        return values.ToArray();
    }

    private List<CarRow> GetFallbackData()
    {
        return new List<CarRow>
        {
            new() { Mileage = 50000, Make = "Volkswagen", Model = "Golf", Fuel = "Gasoline", Gear = "Manual", OfferType = "Used", Price = 18000, Hp = 150, Year = 2019 },
            new() { Mileage = 30000, Make = "BMW", Model = "3 Series", Fuel = "Diesel", Gear = "Automatic", OfferType = "Used", Price = 28000, Hp = 190, Year = 2020 },
            new() { Mileage = 40000, Make = "Audi", Model = "A4", Fuel = "Diesel", Gear = "Automatic", OfferType = "Used", Price = 25000, Hp = 150, Year = 2019 },
            new() { Mileage = 60000, Make = "Mercedes-Benz", Model = "C-Class", Fuel = "Diesel", Gear = "Automatic", OfferType = "Used", Price = 22000, Hp = 170, Year = 2018 },
            new() { Mileage = 20000, Make = "Toyota", Model = "Corolla", Fuel = "Gasoline", Gear = "Manual", OfferType = "Used", Price = 16000, Hp = 132, Year = 2020 },
            new() { Mileage = 45000, Make = "Ford", Model = "Focus", Fuel = "Gasoline", Gear = "Manual", OfferType = "Used", Price = 14000, Hp = 125, Year = 2019 },
            new() { Mileage = 35000, Make = "Skoda", Model = "Octavia", Fuel = "Diesel", Gear = "Automatic", OfferType = "Used", Price = 20000, Hp = 150, Year = 2020 },
            new() { Mileage = 55000, Make = "Hyundai", Model = "Tucson", Fuel = "Diesel", Gear = "Automatic", OfferType = "Used", Price = 19000, Hp = 136, Year = 2019 },
            new() { Mileage = 25000, Make = "Renault", Model = "Clio", Fuel = "Gasoline", Gear = "Manual", OfferType = "Used", Price = 12000, Hp = 100, Year = 2020 },
            new() { Mileage = 15000, Make = "Tesla", Model = "Model 3", Fuel = "Electric", Gear = "Automatic", OfferType = "Used", Price = 38000, Hp = 283, Year = 2021 },
        };
    }

    private List<CarRow> ApplyFilters(List<CarRow> cars, CarRecommendationFilter filter)
    {
        var query = cars.AsEnumerable();

        query = query.Where(c => c.Price <= filter.MaxBudget);
        query = query.Where(c => c.Year >= filter.MinYear);

        if (filter.FuelTypes.Any())
            query = query.Where(c => filter.FuelTypes.Contains(c.Fuel));

        if (filter.Gear != null)
            query = query.Where(c => c.Gear == filter.Gear);

        if (filter.MinHp > 0)
            query = query.Where(c => c.Hp >= filter.MinHp);

        if (filter.MaxHp > 0)
            query = query.Where(c => c.Hp <= filter.MaxHp);

        if (filter.OfferType != "Any")
            query = query.Where(c => c.OfferType == filter.OfferType);

        if (filter.OfferType != "New" && filter.MaxMileage > 0)
            query = query.Where(c => c.Mileage <= filter.MaxMileage);

        return query.ToList();
    }

    private string BuildPrompt(CarRecommendationFilter filter, List<CandidateInfo> candidates)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a car buying advisor for the European used car market.");
        sb.AppendLine("Respond ONLY with a valid JSON array. No markdown, no explanation outside the JSON.");
        sb.AppendLine();
        sb.AppendLine("The buyer's preferences:");
        sb.AppendLine($"- Max budget: €{filter.MaxBudget:N0}");
        sb.AppendLine($"- Min year: {filter.MinYear}");
        sb.AppendLine($"- Fuel: {(filter.FuelTypes.Any() ? string.Join(", ", filter.FuelTypes) : "Any")}");
        sb.AppendLine($"- Transmission: {filter.Gear ?? "Any"}");
        sb.AppendLine($"- Horsepower: {filter.MinHp}–{filter.MaxHp} HP");
        sb.AppendLine($"- Preferred body style: {filter.PreferredBodyHint ?? "No preference"}");
        sb.AppendLine($"- Min seats: {(filter.MinSeatsHint > 0 ? filter.MinSeatsHint.ToString() : "No preference")}");
        sb.AppendLine($"- Condition: {filter.OfferType}");
        sb.AppendLine();
        sb.AppendLine("Candidate cars from our database (ranked by availability):");

        for (int i = 0; i < candidates.Count; i++)
        {
            var c = candidates[i];
            sb.AppendLine($"{i + 1}. {c.Make} {c.Model} | Body: {c.BodyType} | Seats: {c.Seats} | Median price: €{c.MedianPrice:N0} | Predicted fair price: €{c.PredictedPrice:N0} | HP: {c.MinHp:N0}–{c.MaxHp:N0}");
        }

        sb.AppendLine();
        sb.AppendLine("Select the best 3–5 options for this buyer. Rank them best-first.");
        sb.AppendLine("For each, write a 2-sentence explanation covering: why it fits their needs, and one notable tradeoff.");
        sb.AppendLine();
        sb.AppendLine("JSON format:");
        sb.AppendLine("[");
        sb.AppendLine("  { \"make\": \"VW\", \"model\": \"Golf\", \"explanation\": \"...\" },");
        sb.AppendLine("  { \"make\": \"Skoda\", \"model\": \"Octavia\", \"explanation\": \"...\" }");
        sb.AppendLine("]");

        return sb.ToString();
    }

    private List<CarRecommendationResult> ParseLlmResponse(string json, List<CandidateInfo> candidates)
    {
        var results = new List<CarRecommendationResult>();

        try
        {
            // Try to extract JSON array from response (LLM may include extra text)
            var startIdx = json.IndexOf('[');
            var endIdx = json.LastIndexOf(']');
            if (startIdx < 0 || endIdx < 0 || endIdx <= startIdx)
                return results;

            var jsonArray = json.AsMemory(startIdx, endIdx - startIdx + 1);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<LlmRecommendation>>(jsonArray.Span, options);

            if (items == null) return results;

            foreach (var item in items.Take(5))
            {
                var candidate = candidates.FirstOrDefault(c =>
                    c.Make.Equals(item.Make, StringComparison.OrdinalIgnoreCase) &&
                    c.Model.Equals(item.Model, StringComparison.OrdinalIgnoreCase));

                if (candidate != null)
                {
                    results.Add(new CarRecommendationResult
                    {
                        Make = candidate.Make,
                        Model = candidate.Model,
                        EstimatedPrice = candidate.PredictedPrice,
                        MinPrice = candidate.MinPrice,
                        MaxPrice = candidate.MaxPrice,
                        DealScore = candidate.DealScore,
                        LlmExplanation = item.Explanation ?? "Recommended based on your criteria."
                    });
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse LLM JSON response");
        }

        return results;
    }

    private static float Median(IEnumerable<float> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count == 0) return 0;
        int mid = sorted.Count / 2;
        return sorted.Count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2f
            : sorted[mid];
    }

    private class CarRow
    {
        public float Mileage { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Fuel { get; set; } = string.Empty;
        public string Gear { get; set; } = string.Empty;
        public string OfferType { get; set; } = string.Empty;
        public float Price { get; set; }
        public float Hp { get; set; }
        public float Year { get; set; }
    }

    private class CandidateInfo
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public float MedianPrice { get; set; }
        public float PredictedPrice { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public DealScore DealScore { get; set; } = new();
        public string BodyType { get; set; } = string.Empty;
        public int Seats { get; set; }
        public float MinHp { get; set; }
        public float MaxHp { get; set; }
        public int Count { get; set; }
    }

    private class LlmRecommendation
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? Explanation { get; set; }
    }
}

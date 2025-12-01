using System.Globalization;
using CarPricePredictor.Web.Data;

namespace CarPricePredictor.Web.Services;

public class CarDataService : ICarDataService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CarDataService> _logger;
    private Dictionary<string, List<string>>? _brandModelsCache;
    private readonly object _lock = new();

    public CarDataService(IWebHostEnvironment environment, ILogger<CarDataService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public Dictionary<string, List<string>> GetBrandModels()
    {
        if (_brandModelsCache != null)
        {
            return _brandModelsCache;
        }

        lock (_lock)
        {
            if (_brandModelsCache != null)
            {
                return _brandModelsCache;
            }

            _brandModelsCache = LoadBrandModelsFromDataset();
            return _brandModelsCache;
        }
    }

    public Task<Dictionary<string, List<string>>> GetBrandModelsAsync()
    {
        return Task.FromResult(GetBrandModels());
    }

    private Dictionary<string, List<string>> LoadBrandModelsFromDataset()
    {
        // Try to load from the dataset file
        var datasetPath = Path.Combine(_environment.ContentRootPath, "..", "CarPricePredictor.ML", "Data", "autoscout24-germany-dataset.csv");
        
        if (File.Exists(datasetPath))
        {
            try
            {
                _logger.LogInformation($"Loading brand/model data from dataset: {datasetPath}");
                return ExtractBrandModelsFromCsv(datasetPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load brand/model data from dataset, falling back to hardcoded data");
            }
        }
        else
        {
            _logger.LogWarning($"Dataset not found at {datasetPath}, using hardcoded brand/model data");
        }

        // Fallback to hardcoded data
        return CarBrandData.BrandModels;
    }

    private Dictionary<string, List<string>> ExtractBrandModelsFromCsv(string csvPath)
    {
        var brandModels = new Dictionary<string, HashSet<string>>();

        using (var reader = new StreamReader(csvPath))
        {
            // Skip header
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = ParseCsvLine(line);
                
                // Based on CarData.cs: [LoadColumn(1)] Make, [LoadColumn(2)] Model
                if (values.Length >= 3)
                {
                    var make = values[1]?.Trim();
                    var model = values[2]?.Trim();

                    if (!string.IsNullOrWhiteSpace(make) && !string.IsNullOrWhiteSpace(model))
                    {
                        if (!brandModels.ContainsKey(make))
                        {
                            brandModels[make] = new HashSet<string>();
                        }
                        brandModels[make].Add(model);
                    }
                }
            }
        }

        // Sort brands alphabetically, but put "Others" at the end
        var sortedBrands = brandModels.Keys
            .OrderBy(brand => brand.Equals("Others", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
            .ThenBy(brand => brand)
            .ToList();

        // Convert HashSet to List and sort models, maintaining brand order
        var result = new Dictionary<string, List<string>>();
        foreach (var brand in sortedBrands)
        {
            result[brand] = brandModels[brand].OrderBy(m => m).ToList();
        }

        _logger.LogInformation($"Loaded {result.Count} brands with {result.Sum(b => b.Value.Count)} total models from dataset");

        return result;
    }

    private string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        var currentValue = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        values.Add(currentValue.ToString());
        return values.ToArray();
    }
}

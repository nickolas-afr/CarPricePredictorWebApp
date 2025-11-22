using Microsoft.ML;
using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public class MLPredictionService : IPredictionService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer? _model;
    private readonly PredictionEngine<MLCarData, MLCarPricePrediction>? _predictionEngine;

    public MLPredictionService(IWebHostEnvironment environment)
    {
        _mlContext = new MLContext(seed: 0);

        // Try to load the model
        var modelPath = Path.Combine(environment.WebRootPath, "MLModels", "CarPriceModel.zip");
        
        if (File.Exists(modelPath))
        {
            using (var stream = File.OpenRead(modelPath))
            {
                _model = _mlContext.Model.Load(stream, out var modelInputSchema);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<MLCarData, MLCarPricePrediction>(_model);
            }
        }
    }

    public PredictionResultModel PredictPrice(CarInputModel input)
    {
        if (_predictionEngine == null)
        {
            return new PredictionResultModel
            {
                PriceStatus = "Model Not Available",
                PredictedPrice = 0,
                MinPrice = 0,
                MaxPrice = 0,
                Difference = 0,
                DifferencePercentage = 0
            };
        }

        // Convert input to ML model format
        var mlInput = new MLCarData
        {
            Make = input.Make,
            Model = input.Model,
            Mileage = input.Mileage,
            Fuel = input.Fuel,
            Gear = input.Gear,
            OfferType = input.OfferType,
            Hp = input.Hp,
            Year = input.Year
        };

        // Make prediction
        var prediction = _predictionEngine.Predict(mlInput);
        
        // Calculate price range (±15% of predicted price)
        float minPrice = prediction.Price * 0.85f;
        float maxPrice = prediction.Price * 1.15f;
        
        // Determine price status
        string priceStatus;
        float difference = input.Price - prediction.Price;
        float differencePercentage = (difference / prediction.Price) * 100;

        if (input.Price < minPrice)
        {
            priceStatus = "Too Low";
        }
        else if (input.Price > maxPrice)
        {
            priceStatus = "Too High";
        }
        else
        {
            priceStatus = "Good Price";
        }

        return new PredictionResultModel
        {
            PriceStatus = priceStatus,
            PredictedPrice = prediction.Price,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Difference = difference,
            DifferencePercentage = differencePercentage
        };
    }

    // Internal ML.NET model classes
    private class MLCarData
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public float Mileage { get; set; }
        public string Fuel { get; set; } = string.Empty;
        public string Gear { get; set; } = string.Empty;
        public float OfferType { get; set; }
        public float Hp { get; set; }
        public float Year { get; set; }
    }

    private class MLCarPricePrediction
    {
        public float Price { get; set; }
    }
}

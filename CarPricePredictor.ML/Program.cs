using Microsoft.ML;
using Microsoft.ML.Data;
using CarPricePredictor.ML.Models;

namespace CarPricePredictor.ML;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Car Price Predictor - ML Model Training ===\n");

        var mlContext = new MLContext(seed: 0);

        // Load data
        Console.WriteLine("Loading training data from CSV...");
        //string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "autoscout24-germany-dataset.csv");
        string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "germany_used_cars_combined.csv");
        
        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"ERROR: Dataset not found at: {dataPath}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        IDataView rawDataView = mlContext.Data.LoadFromTextFile<CarData>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ',');

        // Pre-process data: Convert absolute Year to CarAge (Dataset is scraped in 2023)
        Console.WriteLine($"Transforming absolute years to relative car age...");
        var processedEnumerable = mlContext.Data.CreateEnumerable<CarData>(rawDataView, reuseRowObject: false)
            .Where(car => !string.IsNullOrWhiteSpace(car.Model))
            .Where(car => car.Hp > 0)
            .Where(car => !string.IsNullOrWhiteSpace(car.Gear))
            .Select(car => 
            {
                return new CarDataAge
                {
                    Mileage = car.Mileage,
                    Make = car.Make,
                    Model = car.Model,
                    Fuel = car.Fuel,
                    Gear = car.Gear,
                    OfferType = car.OfferType,
                    Price = car.Price,
                    Hp = car.Hp,
                    Year = car.Year,
                    CarAge = 2023 - car.Year
                };
            });
        
        IDataView dataView = mlContext.Data.LoadFromEnumerable(processedEnumerable);

        Console.WriteLine($"Loaded data successfully!");
        
        // Inspect the data
        Console.WriteLine("\nInspecting first few rows...");
        var dataPreview = mlContext.Data.CreateEnumerable<CarDataAge>(dataView, reuseRowObject: false).Take(3);
        foreach (var row in dataPreview)
        {
            Console.WriteLine($"  {row.Make} {row.Model}, {row.Year} ({row.CarAge} yrs old), {row.Mileage:N0}km, {row.Hp}hp, ${row.Price:N0}");
        }

        Console.WriteLine("\nFiltering outliers and preparing full dataset for training...");
        // Remove massive outliers that skew the model
        var filteredData = mlContext.Data.FilterRowsByColumn(dataView, "Label", lowerBound: 500, upperBound: 250000);
        filteredData = mlContext.Data.FilterRowsByColumn(filteredData, "Mileage", lowerBound: 0, upperBound: 600000);
        filteredData = mlContext.Data.FilterRowsByColumn(filteredData, "Hp", lowerBound: 20, upperBound: 1000);
        
        Console.WriteLine($"Data ready for training!");

        // Split data for training and testing
        var split = mlContext.Data.TrainTestSplit(filteredData, testFraction: 0.1);

        // Build training pipeline
        Console.WriteLine("Building training pipeline...");
        var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("MakeEncoded", "Make")
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("ModelEncoded", "Model"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("FuelEncoded", "Fuel"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("GearEncoded", "Gear"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("OfferTypeEncoded", "OfferType"))
            .Append(mlContext.Transforms.NormalizeMinMax("MileageNorm", "Mileage"))
            .Append(mlContext.Transforms.NormalizeMinMax("HpNorm", "Hp"))
            .Append(mlContext.Transforms.NormalizeMinMax("CarAgeNorm", "CarAge"))
            .Append(mlContext.Transforms.Concatenate("Features", 
                "MakeEncoded", "ModelEncoded", "MileageNorm", "FuelEncoded", "GearEncoded", "OfferTypeEncoded", "HpNorm", "CarAgeNorm"))
            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 150,                  // Reduced to prevent overfitting (memorizing the dataset)
                numberOfTrees: 1000,                  // Sweet spot for solid boosting without extreme computational cost
                minimumExampleCountPerLeaf: 50,       // Requires at least 50 similar cars to form a rule. Excellent for generalization!
                learningRate: 0.05));                 // Standard balanced learning rate

        // Train the model
        Console.WriteLine("Training model... (this may take a few minutes)");
        var model = pipeline.Fit(split.TrainSet);

        // Evaluate the model
        Console.WriteLine("\nEvaluating model...");
        var predictions = model.Transform(split.TestSet);
        var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

        Console.WriteLine($"\n=== Model Metrics ===");
        Console.WriteLine($"R-Squared: {metrics.RSquared:0.####}");
        Console.WriteLine($"Root Mean Squared Error: ${metrics.RootMeanSquaredError:N2}");
        Console.WriteLine($"Mean Absolute Error: ${metrics.MeanAbsoluteError:N2}");

        // Save the model
        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CarPriceModel.zip");
        Console.WriteLine($"\nSaving model to: {modelPath}");
        mlContext.Model.Save(model, dataView.Schema, modelPath);

        Console.WriteLine("\n✓ Model training completed successfully!");
        Console.WriteLine($"\nNext steps:");
        Console.WriteLine($"1. Copy the model file to your Web project:");
        Console.WriteLine($"   {modelPath}");
        Console.WriteLine($"   → CarPricePredictor.Web/wwwroot/MLModels/CarPriceModel.zip");

        // Test predictions
        Console.WriteLine("\n=== Testing Sample Predictions ===");
        var predictionEngine = mlContext.Model.CreatePredictionEngine<CarDataAge, CarPricePrediction>(model);

        var testCar = new CarDataAge
        {
            Make = "Audi",
            Model = "A4",
            Mileage = 50000,
            Fuel = "Diesel",
            Gear = "Automatic",
            OfferType = "Used",
            Hp = 150,
            Year = 2018,
            CarAge = 2026 - 2018
        };

        var prediction = predictionEngine.Predict(testCar);
        Console.WriteLine($"\nTest Car: {testCar.Make} {testCar.Model}, {testCar.Year}, {testCar.Mileage:N0}km, {testCar.Hp}hp");
        Console.WriteLine($"Predicted Price: ${prediction.Price:N2}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
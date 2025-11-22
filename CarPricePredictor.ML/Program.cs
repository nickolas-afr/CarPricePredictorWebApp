using Microsoft.ML;
using Microsoft.ML.Data;
using CarPricePredictor.ML.Models;

namespace CarPricePredictor.ML;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Car Price Predictor - ML Model Training ===\n");
        Console.WriteLine("Using Kaggle Cars Germany Dataset\n");

        var mlContext = new MLContext(seed: 0);

        // Load data
        Console.WriteLine("Loading training data from CSV...");
        string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "autoscout24-germany-dataset.csv");
        
        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"ERROR: Dataset not found at: {dataPath}");
            Console.WriteLine("
Please download the dataset from:");
            Console.WriteLine("https://www.kaggle.com/datasets/ander289386/cars-germany");
            Console.WriteLine("
Extract and place 'autoscout24-germany-dataset.csv' in the Data folder.");
            Console.WriteLine("
Press any key to exit...");
            Console.ReadKey();
            return;
        }

        IDataView dataView = mlContext.Data.LoadFromTextFile<CarData>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ',');

        // Take a sample for faster training (optional - remove for full dataset)
        var preview = mlContext.Data.TakeRows(dataView, 50000);
        
        Console.WriteLine($"Loaded data successfully!");

        // Split data for training and testing
        var split = mlContext.Data.TrainTestSplit(preview, testFraction: 0.2);

        // Build training pipeline
        Console.WriteLine("Building training pipeline...");
        var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("MakeEncoded", "Make")
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("ModelEncoded", "Model"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("FuelEncoded", "Fuel"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding("GearEncoded", "Gear"))
            .Append(mlContext.Transforms.Concatenate("Features", 
                "MakeEncoded", "ModelEncoded", "Mileage", "FuelEncoded", "GearEncoded", "OfferType", "Hp", "Year"))
            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 10,
                learningRate: 0.2));

        // Train the model
        Console.WriteLine("Training model... (this may take a few minutes)");
        var model = pipeline.Fit(split.TrainSet);

        // Evaluate the model
        Console.WriteLine("\nEvaluating model...");
        var predictions = model.Transform(split.TestSet);
        var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

        Console.WriteLine($"\n=== Model Metrics ===");
        Console.WriteLine($"R-Squared: {metrics.RSquared:0.####}");
        Console.WriteLine($"Root Mean Squared Error: €{metrics.RootMeanSquaredError:N2}");
        Console.WriteLine($"Mean Absolute Error: €{metrics.MeanAbsoluteError:N2}");

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
        var predictionEngine = mlContext.Model.CreatePredictionEngine<CarData, CarPricePrediction>(model);

        var testCar = new CarData
        {
            Make = "Audi",
            Model = "A4",
            Mileage = 50000,
            Fuel = "Diesel",
            Gear = "Automatic",
            OfferType = 0,
            Hp = 150,
            Year = 2018
        };

        var prediction = predictionEngine.Predict(testCar);
        Console.WriteLine($"\nTest Car: {testCar.Make} {testCar.Model}, {testCar.Year}, {testCar.Mileage:N0}km, {testCar.Hp}hp");
        Console.WriteLine($"Predicted Price: €{prediction.Price:N2}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
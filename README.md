# Car Price Predictor Web App

A web application built with ASP.NET Core Blazor Server and ML.NET that predicts whether a car's price is fair, too high, or too low based on its specifications.

## Features

- **Price Analysis**: Input car details and get instant feedback on pricing
- **ML-Powered**: Uses ML.NET for predictions based on real car market data
- **Price Range**: Shows typical price range for similar cars
- **User-Friendly**: Clean Blazor Server interface for easy interaction

## Technologies

- **ASP.NET Core 8.0**: Web framework
- **Blazor Server**: Interactive web UI
- **ML.NET 3.0.1**: Machine learning framework
- **Bootstrap 5**: Responsive UI design

## Project Structure

- `CarPricePredictor.ML`: Console application for training the ML model
- `CarPricePredictor.Web`: Blazor Server web application
- `Components/Pages/Predict.razor`: Main prediction interface

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code (optional)

### Step 1: Download Dataset

1. Visit [Kaggle Cars Germany Dataset](https://www.kaggle.com/datasets/ander289386/cars-germany)
2. Download and extract `autoscout24-germany-dataset.csv`
3. Place it in `CarPricePredictor.ML/Data/`

### Step 2: Train the Model

```bash
cd CarPricePredictor.ML
dotnet run
```

This will:
- Load the dataset
- Train a FastTree regression model
- Save the model to `CarPriceModel.zip`

### Step 3: Copy Model to Web Project

Copy the generated `CarPriceModel.zip` file to:
```
CarPricePredictor.Web/wwwroot/MLModels/CarPriceModel.zip
```

### Step 4: Run the Web Application

```bash
cd CarPricePredictor.Web
dotnet run
```

Or run the entire solution:
```bash
dotnet run --project CarPricePredictor.csproj
```

Visit `https://localhost:5001` or `http://localhost:5000` in your browser.

## Usage

1. Navigate to the "Price Prediction" page
2. Enter car details:
   - Brand (Make)
   - Model
   - Production Year
   - Mileage (km)
   - Fuel Type
   - Transmission
   - Horsepower
   - Offer Type (New/Used)
   - Asking Price
3. Click "Check Price"
4. View the prediction result:
   - **Good Price**: Within fair market range (±15%)
   - **Too High**: Above market value
   - **Too Low**: Below market value

## Model Details

The ML model uses:
- **Algorithm**: FastTree Regression
- **Features**: Make, Model, Mileage, Fuel Type, Transmission, Horsepower, Year
- **Target**: Price prediction in Euros
- **Accuracy**: R-Squared and RMSE metrics shown during training

## Price Determination Logic

- **Good Price**: Within 85%-115% of predicted price
- **Too High**: Above 115% of predicted price
- **Too Low**: Below 85% of predicted price

## License

MIT License - feel free to use and modify as needed.

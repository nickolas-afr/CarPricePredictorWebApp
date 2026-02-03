# Car Price Predictor Web App

A web application built with ASP.NET Core Blazor Server and ML.NET that predicts whether a car's price is fair, too high, or too low based on its specifications.

## Features

### Core Features
- **Price Analysis**: Input car details and get instant feedback on pricing
- **ML-Powered**: Uses ML.NET for predictions based on real car market data
- **Price Range**: Shows typical price range for similar cars
- **User-Friendly**: Clean Blazor Server interface for easy interaction

### New Features

#### 🔍 VIN Decoder
Quickly populate car details by entering a VIN (Vehicle Identification Number).
- 17-character VIN validation (no I, O, or Q characters)
- Auto-populates: Make, Model, Year, Trim, Engine, Transmission, Fuel Type, Body Type
- **Uses RapidAPI VIN Decoder Europe2** - Optimized for European cars
- Decodes European VIN numbers with accurate data
- Shows success/error feedback
- Saves time on data entry

#### 🌙 Dark Mode
Switch between light and dark themes for comfortable viewing.
- Toggle button in the navigation bar
- Persists preference in browser localStorage
- Respects system preference (prefers-color-scheme) as default
- Smooth transitions between themes
- All components optimized for both themes

#### 📊 Deal Score Gauge
Visual gauge showing whether you're getting a good deal.
- Semi-circular Chart.js gauge visualization
- Score calculation based on:
  - Predicted price vs. asking price
  - Market average comparison (when available)
- Color-coded zones:
  - 🟢 Green (70-100%): Good Deal / Great Deal
  - 🟡 Yellow (30-70%): Fair Price / Slightly High
  - 🔴 Red (0-30%): Overpriced / Very Overpriced
- Descriptive labels and recommendations

## Technologies

- **ASP.NET Core 10.0**: Web framework
- **Blazor Server**: Interactive web UI
- **ML.NET 5.0.0**: Machine learning framework
- **Bootstrap 5.3.8**: Responsive UI design
- **Chart.js 4.4.1**: Deal score gauge visualization
- **RapidAPI VIN Decoder Europe2**: European VIN decoding

## Project Structure

- `CarPricePredictor.ML`: Console application for training the ML model
- `CarPricePredictor.Web`: Blazor Server web application
- `Components/Pages/Predict.razor`: Main prediction interface

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2026 or VS Code (optional)

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

### Price Prediction

1. Navigate to the "Price Prediction" page
2. **(Optional)** Enter a 17-character VIN and click "Decode VIN" to auto-populate fields
3. Enter or verify car details:
   - Brand (Make)
   - Model
   - Production Year
   - Mileage (km)
   - Fuel Type
   - Transmission
   - Horsepower
   - Offer Type (New/Used)
   - Asking Price
4. Click "Check Price"
5. View comprehensive results:
   - **Deal Score Gauge**: Visual indicator of deal quality
   - **Price Analysis**: Predicted price vs. asking price
   - **Price Range**: Typical market range for similar cars
   - **Recommendation**: Clear verdict on the deal

### Dark Mode

- Click the 🌙/☀️ icon in the top navigation bar to toggle between light and dark themes
- Your preference is automatically saved
- Theme respects your system preference by default

## Price Determination Logic

### ML Model Prediction
- **Algorithm**: FastTree Regression
- **Features**: Make, Model, Mileage, Fuel Type, Transmission, Horsepower, Year
- **Target**: Price prediction in Euros
- **Accuracy**: R-Squared and RMSE metrics shown during training
- **Price Classification**:
  - **Good Price**: Within 85%-115% of predicted price
  - **Too High**: Above 115% of predicted price
  - **Too Low**: Below 85% of predicted price

### Deal Score Calculation
The deal score (0-100) is calculated by comparing the asking price to the predicted price:
- **70-100 (Green)**: Great Deal / Good Deal
- **30-70 (Yellow)**: Fair Price / Slightly High
- **0-30 (Red)**: Overpriced / Very Overpriced

## API Integration

### RapidAPI VIN Decoder Europe2
The application uses **RapidAPI VIN Decoder Europe2** for VIN decoding, optimized for European cars:
- **Requires API key** - Sign up at [RapidAPI](https://rapidapi.com/hub)
- **Free tier available** - Suitable for personal projects with reasonable limits
- Provides accurate vehicle data for European cars
- Better coverage for European manufacturers (BMW, Mercedes, Audi, etc.)

**Endpoint Used:**
- `GET https://vin-decoder-europe2.p.rapidapi.com/vin_decoder?vin={vin}`
- Headers: `x-rapidapi-key`, `x-rapidapi-host`

**API Setup:**
1. Visit [RapidAPI VIN Decoder Europe2](https://rapidapi.com/dataproviders/api/vin-decoder-europe2)
2. Subscribe to the free plan
3. Copy your API key
4. Configure in appsettings.json or environment variables (see Configuration section below)

**API Documentation:** [RapidAPI VIN Decoder Europe2](https://rapidapi.com/dataproviders/api/vin-decoder-europe2)

## Configuration

### appsettings.json Structure
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "VinDecoder": {
    "BaseUrl": "https://vin-decoder-europe2.p.rapidapi.com",
    "ApiHost": "vin-decoder-europe2.p.rapidapi.com",
    "ApiKey": "your-rapidapi-key-here",
    "UseMockData": false
  }
}
```

### Configuration Options
- `VinDecoder:BaseUrl`: RapidAPI endpoint (default: https://vin-decoder-europe2.p.rapidapi.com)
- `VinDecoder:ApiHost`: RapidAPI host header (default: vin-decoder-europe2.p.rapidapi.com)
- `VinDecoder:ApiKey`: Your RapidAPI key (required for real VIN decoding)
- `VinDecoder:UseMockData`: Set to `true` to use mock data instead of real API (for testing)

### Environment Variables (Production)
For production deployments, use environment variables:
```bash
export VinDecoder__ApiKey="your-rapidapi-key-here"
export VinDecoder__ApiHost="vin-decoder-europe2.p.rapidapi.com"
export VinDecoder__BaseUrl="https://vin-decoder-europe2.p.rapidapi.com"
```

### User Secrets (Recommended for Development)
```bash
cd CarPricePredictor.Web
dotnet user-secrets set "VinDecoder:ApiKey" "your-rapidapi-key-here"
```

## Architecture

### Project Structure
```
CarPricePredictorWebApp/
├── CarPricePredictor.ML/          # ML model training console app
│   ├── Data/                       # Training datasets
│   └── Program.cs                  # Model training logic
├── CarPricePredictor.Web/         # Blazor Server web app
│   ├── Components/
│   │   ├── Layout/                 # Layout components
│   │   ├── Pages/                  # Page components
│   │   └── DealScoreGauge.razor   # Deal score visualization
│   ├── Models/                     # Data models
│   ├── Services/                   # Business logic services
│   │   ├── MLPredictionService.cs
│   │   ├── VinDecoderService.cs
│   │   └── DealScoreService.cs
│   └── wwwroot/
│       ├── css/site.css           # Themed styles
│       └── js/theme.js            # Dark mode toggle
└── README.md
```

### Services
- **IPredictionService**: ML.NET price prediction
- **IVinDecoderService**: VIN decoding via RapidAPI VIN Decoder Europe2
- **IDealScoreService**: Deal score calculation
- **ICarDataService**: Car brand/model data

## Dependencies

### NuGet Packages
- `Microsoft.ML` (5.0.0) - Machine learning framework
- `Microsoft.ML.FastTree` (5.0.0) - FastTree algorithm

### CDN Resources
- Bootstrap 5.3.8 - UI framework
- Bootstrap Icons 1.11.3 - Icon library
- Chart.js 4.4.1 - Gauge visualization

## Troubleshooting

### ML Model Not Found
If you see "Model Not Available" error:
1. Ensure you've trained the model using `CarPricePredictor.ML`
2. Copy `CarPriceModel.zip` to `CarPricePredictor.Web/wwwroot/MLModels/`
3. Restart the application

### API Features Not Working
If VIN decoder shows errors:
1. Verify you have a valid RapidAPI key configured in `VinDecoder:ApiKey`
2. Check internet connectivity (RapidAPI requires internet access)
3. Ensure you haven't exceeded your RapidAPI rate limits
4. Verify the VIN is valid (17 characters, no I/O/Q)
5. Try enabling mock data mode by setting `VinDecoder:UseMockData` to `true` in appsettings.json
6. Review application logs for detailed error messages

**Note:** Without a valid API key, the application will automatically use mock data for demonstrations.

### Dark Mode Not Persisting
If theme doesn't persist between sessions:
1. Check browser localStorage is enabled
2. Clear browser cache and try again
3. Check browser console for JavaScript errors

## License

MIT License - feel free to use and modify as needed.

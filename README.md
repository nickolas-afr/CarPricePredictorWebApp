# Car Price Predictor Web App

A web application built with ASP.NET Core Blazor Server and ML.NET that predicts whether a car's price is fair, too high, or too low based on its specifications.

## Features

### Core Features
- **Price Analysis**: Input car details and get instant feedback on pricing
- **ML-Powered**: Uses ML.NET for predictions based on real car market data
- **Price Range**: Shows typical price range for similar cars
- **User-Friendly**: Clean Blazor Server interface for easy interaction

### New Features

#### 🌐 Market Comparison
Compare your car's predicted price against real market listings from European marketplaces.
- Fetches similar vehicles from AutoScout24, Mobile.de, and other sources via Carapis API
- Shows 3-5 similar listings with prices and sources
- Displays average market price
- Compares your asking price to market average with percentage difference
- Automatically matches based on make, model, year (±2 years), and mileage (±15,000 km)

#### 🔍 VIN Decoder
Quickly populate car details by entering a VIN (Vehicle Identification Number).
- 17-character VIN validation (no I, O, or Q characters)
- Auto-populates: Make, Model, Year, Trim, Engine, Transmission, Fuel Type, Body Type
- Integrates with Carapis VIN decoder API
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
- **Carapis API**: Market data and VIN decoding

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

### Step 4: Configure Carapis API (Optional)

The application uses the Carapis API for market comparison and VIN decoding features. While the app works without an API key (using mock data), you can get real market data by:

1. **Get an API Key:**
   - Visit [Carapis.com](https://carapis.com) and sign up for an account
   - Generate an API key from your dashboard
   - Free tier available for testing

2. **Configure the API Key:**
   
   **Option A - appsettings.json (Development):**
   ```json
   {
     "Carapis": {
       "ApiKey": "your-actual-api-key-here",
       "BaseUrl": "https://api.carapis.com/v1"
     }
   }
   ```

   **Option B - Environment Variables (Production):**
   ```bash
   export Carapis__ApiKey="your-actual-api-key-here"
   export Carapis__BaseUrl="https://api.carapis.com/v1"
   ```

   **Option C - User Secrets (Recommended for Development):**
   ```bash
   cd CarPricePredictor.Web
   dotnet user-secrets set "Carapis:ApiKey" "your-actual-api-key-here"
   ```

3. **Restart the Application** for changes to take effect.

**Note:** Without a valid API key, the application will use mock data for demonstrations. All features will still work, but market comparison data will be simulated.

### Step 5: Run the Web Application

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
   - **Market Comparison**: Real listings from European marketplaces
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
The deal score (0-100) is calculated by comparing the asking price to the predicted price and market average:
- **70-100 (Green)**: Great Deal / Good Deal
- **30-70 (Yellow)**: Fair Price / Slightly High
- **0-30 (Red)**: Overpriced / Very Overpriced

## API Integration

### Carapis API
The application integrates with Carapis for:
- **Market Comparison**: Fetches real-time listings from European marketplaces
- **VIN Decoding**: Decodes VIN numbers to auto-populate car details

**Endpoints Used:**
- `GET /v1/listings/search` - Search for similar car listings
- `GET /v1/vin/decode/{vin}` - Decode VIN number

**API Documentation:** [Carapis API Docs](https://carapis.com/docs)

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
  "Carapis": {
    "ApiKey": "your-carapis-api-key-here",
    "BaseUrl": "https://api.carapis.com/v1"
  }
}
```

### Environment Variables
For production deployments, use environment variables:
- `Carapis__ApiKey`: Your Carapis API key
- `Carapis__BaseUrl`: Carapis API base URL (default: https://api.carapis.com/v1)

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
│   │   ├── MarketComparisonService.cs
│   │   ├── VinDecoderService.cs
│   │   └── DealScoreService.cs
│   └── wwwroot/
│       ├── css/site.css           # Themed styles
│       └── js/theme.js            # Dark mode toggle
└── README.md
```

### Services
- **IPredictionService**: ML.NET price prediction
- **IMarketComparisonService**: Carapis market data integration
- **IVinDecoderService**: VIN decoding and validation
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
If market comparison or VIN decoder shows errors:
1. Check that you have a valid Carapis API key configured
2. Verify internet connectivity
3. Check API key hasn't exceeded rate limits
4. Review application logs for detailed error messages

### Dark Mode Not Persisting
If theme doesn't persist between sessions:
1. Check browser localStorage is enabled
2. Clear browser cache and try again
3. Check browser console for JavaScript errors

## License

MIT License - feel free to use and modify as needed.

using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public class DealScoreService : IDealScoreService
{
    public DealScore CalculateDealScore(float predictedPrice, float askingPrice, float? marketAveragePrice = null)
    {
        var score = new DealScore();

        if (askingPrice <= 0 || predictedPrice <= 0)
        {
            score.Score = 50;
            score.Label = "Unknown";
            score.Description = "Insufficient data to calculate deal score";
            score.ColorZone = "yellow";
            return score;
        }

        // Use market average if available, otherwise use predicted price
        var referencePrice = marketAveragePrice ?? predictedPrice;

        // Calculate how good the deal is
        // Lower asking price compared to reference = better deal (higher score)
        var ratio = askingPrice / referencePrice;

        if (ratio <= 0.85f) // 15% or more below reference
        {
            score.Score = 100;
            score.Label = "Great Deal! 🎉";
            score.Description = "This is an excellent price!";
            score.ColorZone = "green";
        }
        else if (ratio <= 0.95f) // 5-15% below reference
        {
            score.Score = 85;
            score.Label = "Good Deal";
            score.Description = "This is a good price";
            score.ColorZone = "green";
        }
        else if (ratio <= 1.05f) // Within 5% of reference
        {
            score.Score = 70;
            score.Label = "Fair Price";
            score.Description = "This is a fair market price";
            score.ColorZone = "yellow";
        }
        else if (ratio <= 1.15f) // 5-15% above reference
        {
            score.Score = 50;
            score.Label = "Slightly High";
            score.Description = "Price is slightly above market value";
            score.ColorZone = "yellow";
        }
        else if (ratio <= 1.25f) // 15-25% above reference
        {
            score.Score = 30;
            score.Label = "Overpriced ⚠️";
            score.Description = "Price is above market value";
            score.ColorZone = "red";
        }
        else // More than 25% above reference
        {
            score.Score = 10;
            score.Label = "Very Overpriced ⚠️";
            score.Description = "Price is significantly above market value";
            score.ColorZone = "red";
        }

        return score;
    }
}

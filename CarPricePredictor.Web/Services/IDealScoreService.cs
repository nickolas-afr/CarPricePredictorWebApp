using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public interface IDealScoreService
{
    DealScore CalculateDealScore(float predictedPrice, float askingPrice, float? marketAveragePrice = null);
}

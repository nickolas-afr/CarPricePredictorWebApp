using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public interface ICarRecommendationService
{
    Task<List<CarRecommendationResult>> GetRecommendationsAsync(CarRecommendationFilter filter);
}

using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public interface IMarketComparisonService
{
    Task<MarketComparisonResult> GetMarketComparisonAsync(CarInputModel input);
}

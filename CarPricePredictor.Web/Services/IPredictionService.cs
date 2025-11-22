using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public interface IPredictionService
{
    PredictionResultModel PredictPrice(CarInputModel input);
}

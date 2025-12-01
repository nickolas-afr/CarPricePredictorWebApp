namespace CarPricePredictor.Web.Services;

public interface ICarDataService
{
    Dictionary<string, List<string>> GetBrandModels();
    Task<Dictionary<string, List<string>>> GetBrandModelsAsync();
}

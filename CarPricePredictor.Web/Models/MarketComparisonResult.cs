namespace CarPricePredictor.Web.Models;

public class MarketComparisonResult
{
    public List<MarketListing> Listings { get; set; } = new();
    public float AverageMarketPrice { get; set; }
    public float PriceComparisonPercentage { get; set; }
    public bool HasData { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

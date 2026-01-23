namespace CarPricePredictor.Web.Models;

public class MarketListing
{
    public string Source { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public float Mileage { get; set; }
    public float Price { get; set; }
    public string Url { get; set; } = string.Empty;
}

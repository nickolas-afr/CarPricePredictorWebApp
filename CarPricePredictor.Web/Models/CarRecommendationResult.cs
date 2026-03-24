namespace CarPricePredictor.Web.Models;

public class CarRecommendationResult
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public float EstimatedPrice { get; set; }
    public float MinPrice { get; set; }
    public float MaxPrice { get; set; }
    public DealScore DealScore { get; set; } = new();
    public string LlmExplanation { get; set; } = string.Empty;
}

namespace CarPricePredictor.Web.Models;

public class PredictionResultModel
{
    public string PriceStatus { get; set; } = string.Empty;
    public float PredictedPrice { get; set; }
    public float MinPrice { get; set; }
    public float MaxPrice { get; set; }
    public float Difference { get; set; }
    public float DifferencePercentage { get; set; }
}
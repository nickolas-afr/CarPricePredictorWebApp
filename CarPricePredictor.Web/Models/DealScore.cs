namespace CarPricePredictor.Web.Models;

public class DealScore
{
    public float Score { get; set; } // 0-100
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ColorZone { get; set; } = string.Empty; // "red", "yellow", "green"
}

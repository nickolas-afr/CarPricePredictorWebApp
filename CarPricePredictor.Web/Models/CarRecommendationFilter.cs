namespace CarPricePredictor.Web.Models;

public class CarRecommendationFilter
{
    public float MaxBudget { get; set; }
    public int MinYear { get; set; }
    public List<string> FuelTypes { get; set; } = new();
    public string? Gear { get; set; }             // null = any
    public float MinHp { get; set; }
    public float MaxHp { get; set; }
    public string OfferType { get; set; } = "Any"; // "Used", "New", "Any"
    public float MaxMileage { get; set; }          // ignored if OfferType = "New"
    public string? PreferredBodyHint { get; set; } // passed to LLM only, not filtered
    public int MinSeatsHint { get; set; }          // passed to LLM only, not filtered
}

namespace CarPricePredictor.Web.Models;

public class CarInputModel
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public float Mileage { get; set; }
    public string Fuel { get; set; } = string.Empty;
    public string Gear { get; set; } = string.Empty;
    public float OfferType { get; set; }
    public float Price { get; set; }
    public float Hp { get; set; }
    public int Year { get; set; }
}
namespace CarPricePredictor.Web.Models;

public class VinDecodeResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    
    // Car details
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Trim { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
}

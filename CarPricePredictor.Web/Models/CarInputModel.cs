using System.ComponentModel.DataAnnotations;

namespace CarPricePredictor.Web.Models;

public class CarInputModel
{
    [Required(ErrorMessage = "Brand is required")]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Production Year is required")]
    [Range(1990, 2025, ErrorMessage = "Production Year must be between 1900 and 2025")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Mileage is required")]
    [Range(0, 1000000, ErrorMessage = "Mileage must be between 0 and 1,000,000 km")]
    public float Mileage { get; set; }

    public string Fuel { get; set; } = string.Empty;
    public string Gear { get; set; } = string.Empty;
    public float Hp { get; set; }

    [Required(ErrorMessage = "Offer Type is required")]
    public float OfferType { get; set; }

    [Required(ErrorMessage = "Asking Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Asking Price must be greater than 0")]
    public float Price { get; set; }
}
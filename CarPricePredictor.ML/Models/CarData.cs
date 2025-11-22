using Microsoft.ML.Data;

namespace CarPricePredictor.ML.Models;

public class CarData
{
    [LoadColumn(0)]
    public string Make { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Model { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float Mileage { get; set; }

    [LoadColumn(3)]
    public string Fuel { get; set; } = string.Empty;

    [LoadColumn(4)]
    public string Gear { get; set; } = string.Empty;

    [LoadColumn(5)]
    public float OfferType { get; set; }

    [LoadColumn(6)]
    [ColumnName("Label")]
    public float Price { get; set; }

    [LoadColumn(7)]
    public float Hp { get; set; }

    [LoadColumn(8)]
    public float Year { get; set; }
}
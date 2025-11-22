using Microsoft.ML.Data;

namespace CarPricePredictor.ML.Models;

public class CarPricePrediction
{
    [ColumnName("Score")]
    public float Price { get; set; }
}
using CarPricePredictor.Web.Models;

namespace CarPricePredictor.Web.Services;

public interface IVinDecoderService
{
    Task<VinDecodeResult> DecodeVinAsync(string vin);
    bool IsValidVin(string vin);
}

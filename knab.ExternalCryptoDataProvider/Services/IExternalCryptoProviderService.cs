using knab.Shared.Models;

namespace knab.Shared.Services
{
    public interface IExternalCryptoProviderService
    {
        Task<Dictionary<string, ExternalCryptoDataProviderCryptoQuote>> GetExternalCryptoDataForCurrencyCodeAsync(string cryptoCurrencyCode);
    }
}

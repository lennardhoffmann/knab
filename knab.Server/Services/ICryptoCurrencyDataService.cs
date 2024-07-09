using knab.ExternalCryptoDataProvider.Models;

namespace knab.API.Services
{
    public interface ICryptoCurrencyDataService
    {
        Task GetCryptoCurrencyProperties();
        Task StoreRequestForCryptoCurrency(string cryptoCurrencyCode, Dictionary<string, ExternalCryptoDataProviderCryptoQuote> externalCryptoResponse);
    }
}

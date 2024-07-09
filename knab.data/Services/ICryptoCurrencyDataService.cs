using knab.DataAccess.Models;
using knab.Shared.Models;

namespace knab.DataAccess.Services
{
    public interface ICryptoCurrencyDataService
    {
        Task<List<CryptoCurrencyProperty>> GetCryptoCurrencyProperties();
        Task StoreRequestForCryptoCurrency(string cryptoCurrencyCode, Dictionary<string, ExternalCryptoDataProviderCryptoQuote> externalCryptoResponse);
    }
}

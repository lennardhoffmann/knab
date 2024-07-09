using knab.ExternalCryptoDataProvider.Models;
using System.Text.Json;

namespace knab.ExternalCryptoDataProvider.Services
{
    public static class CryptoDataExtractionService
    {
        public static ExternalCryptoDataProviderResponse DeserializeCryptoData(string jsonResponse)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<ExternalCryptoDataProviderResponse>(jsonResponse, options);
        }

        public static Dictionary<string, ExternalCryptoDataProviderCryptoQuote> ExtractQuotes(ExternalCryptoDataProviderResponse cryptoResponse, string cryptoCurrencyCode)
        {
            var quotes = new Dictionary<string, ExternalCryptoDataProviderCryptoQuote>();

            if (cryptoResponse.Data.TryGetValue(cryptoCurrencyCode, out var cryptoDataList))
            {
                foreach (var cryptoData in cryptoDataList)
                {
                    foreach (var quote in cryptoData.Quote)
                    {
                        quotes[quote.Key] = quote.Value;
                    }
                }
            }

            return quotes;
        }
    }
}

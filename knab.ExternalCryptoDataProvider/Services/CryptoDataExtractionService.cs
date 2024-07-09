using knab.Shared.Models;
using System.Text.Json;

namespace knab.Shared.Services
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

            if (cryptoResponse.Data.TryGetValue(cryptoCurrencyCode, out var cryptoData))
            {
                foreach (var quote in cryptoData.Quote)
                {
                    quotes[quote.Key] = new ExternalCryptoDataProviderCryptoQuote
                    {
                        Price = quote.Value.Price,
                        LastUpdated = quote.Value.LastUpdated
                    };
                }
            }

            return quotes;
        }
    }
}

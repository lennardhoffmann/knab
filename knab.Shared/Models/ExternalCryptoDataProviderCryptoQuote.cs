using System.Text.Json.Serialization;

namespace knab.Shared.Models
{
    public class ExternalCryptoDataProviderCryptoQuote
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }
    }

    public class ExternalCryptoDataProviderCryptoQuotes
    {
        public Dictionary<string, ExternalCryptoDataProviderCryptoQuote> Quote { get; set; }
    }
}

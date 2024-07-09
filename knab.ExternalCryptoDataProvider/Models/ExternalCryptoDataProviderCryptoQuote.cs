namespace knab.ExternalCryptoDataProvider.Models
{
    public class ExternalCryptoDataProviderCryptoQuote
    {
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ExternalCryptoDataProviderCryptoQuotes
    {
        public Dictionary<string, ExternalCryptoDataProviderCryptoQuote> Quote { get; set; }
    }
}

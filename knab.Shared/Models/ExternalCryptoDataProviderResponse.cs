namespace knab.Shared.Models
{
    public class ExternalCryptoDataProviderResponse
    {
        public Dictionary<string, ExternalCryptoDataProviderCryptoQuotes> Data { get; set; }
    }

    public class ExternalCryptoDataProviderCryptoQuotesWrapper
    {
        public ExternalCryptoDataProviderCryptoQuotes Quote { get; set; }
    }
}

namespace knab.API.Models
{
    public class ExternalCryptoProviderSettings
    {
        public string Name { get; set; }
        public string BaseURL { get; set; }
        public string ApiKey { get; set; }
        public string? Headers { get; set; }
        public string? RequiredCurrencies { get; set; }
    }
}

using knab.DataAccess.Services;
using knab.Shared.Models;
using Microsoft.Extensions.Logging;

namespace knab.Shared.Services
{
    public class ExternalCryptoProviderService : IExternalCryptoProviderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CryptoDataProviderSettingsService _settingsService;
        private readonly CryptoCurrencyPropertyService _propertyService;
        private readonly ILogger<ExternalCryptoProviderService> _logger;

        public ExternalCryptoProviderService(
            IHttpClientFactory httpClientFactory,
            CryptoDataProviderSettingsService settingsService,
            CryptoCurrencyPropertyService propertyService,
            ILogger<ExternalCryptoProviderService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settingsService = settingsService;
            _propertyService = propertyService;
            _logger = logger;
        }

        public async Task<Dictionary<string, ExternalCryptoDataProviderCryptoQuote>> GetExternalCryptoDataForCurrencyCodeAsync(string cryptoCurrencyCode)
        {
            var currencyProperties = (await _propertyService.GetCryptoCurrencyPropertiesAsync()).FirstOrDefault(x => x.Symbol == cryptoCurrencyCode) ?? throw new Exception($"Invalid crypto currency code provided: {cryptoCurrencyCode}");
            try
            {
                var request = await BuildHttpClientRequestForData(currencyProperties.Slug);
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.SendAsync(request, cancellationToken: default);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                var deserializedResponse = CryptoDataExtractionService.DeserializeCryptoData(data);
                var structuredResponse = CryptoDataExtractionService.ExtractQuotes(deserializedResponse, currencyProperties.Slug);

                return structuredResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while making an HTTP request.");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "The HTTP request timed out.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching external crypto data.");
                throw new Exception("An unexpected error occurred while fetching external crypto data.", ex);
            }
        }

        private async Task<HttpRequestMessage> BuildHttpClientRequestForData(string cryptoCurrencySlug)
        {
            var providerSettings = await _settingsService.GetCryptoProviderSettingsAsync();
            var baseAddress = new Uri($"{providerSettings.BaseUrl}/quotes/latest?slug={cryptoCurrencySlug}&convert={providerSettings.RequiredCurrencies}");
            var request = new HttpRequestMessage(HttpMethod.Get, baseAddress);

            if (!string.IsNullOrEmpty(providerSettings.Headers))
            {
                request.Headers.Add(providerSettings.Headers, providerSettings.ApiKey);
            }

            return request;
        }
    }
}

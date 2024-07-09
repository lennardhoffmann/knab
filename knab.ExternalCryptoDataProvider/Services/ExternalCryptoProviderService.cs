using knab.ExternalCryptoDataProvider.Models;
using Microsoft.Extensions.Logging;

namespace knab.ExternalCryptoDataProvider.Services
{
    public class ExternalCryptoProviderService : IExternalCryptoProviderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CryptoDataProviderSettingsService _settingsService;
        private readonly ILogger<ExternalCryptoProviderService> _logger;

        public ExternalCryptoProviderService(IHttpClientFactory httpClientFactory, CryptoDataProviderSettingsService settingsService, ILogger<ExternalCryptoProviderService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settingsService = settingsService;
            _logger = logger;
        }

        public async Task<Dictionary<string, ExternalCryptoDataProviderCryptoQuote>> GetExternalCryptoDataForCurrencyCodeAsync(string cryptoCurrencyCode)
        {
            try
            {
                var request = await BuildHttpClientRequestForData(cryptoCurrencyCode);
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.SendAsync(request, cancellationToken: default);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                var deserializedResponse = CryptoDataExtractionService.DeserializeCryptoData(data);
                var structuredResponse = CryptoDataExtractionService.ExtractQuotes(deserializedResponse, cryptoCurrencyCode);

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

        private async Task<HttpRequestMessage> BuildHttpClientRequestForData(string cryptoCurrencyCode)
        {
            var providerSettings = await _settingsService.GetCryptoProviderSettingsAsync();
            var baseAddress = new Uri($"{providerSettings.BaseUrl}/quotes/latest?convert={providerSettings.RequiredCurrencies}&symbol={cryptoCurrencyCode}");
            var request = new HttpRequestMessage(HttpMethod.Get, baseAddress);

            if (!string.IsNullOrEmpty(providerSettings.Headers))
            {
                request.Headers.Add(providerSettings.Headers, providerSettings.ApiKey);
            }

            return request;
        }
    }
}

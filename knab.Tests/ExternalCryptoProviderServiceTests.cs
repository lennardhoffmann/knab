using FluentAssertions;
using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.ExternalCryptoDataProvider.Models;
using knab.ExternalCryptoDataProvider.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace knab.ExternalCryptoDataProvider.Tests
{
    public class ExternalCryptoProviderServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<ICryptoDataProviderSettingsRepository> _mockSettingsRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<ExternalCryptoProviderService>> _mockLogger;
        private readonly Mock<ILogger<CryptoDataProviderSettingsService>> _mockLoggerSettings;

        private readonly MemoryCache _memoryCache;
        private readonly ExternalCryptoProviderService _sut;

        public ExternalCryptoProviderServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClient = new();
            _mockSettingsRepository = new Mock<ICryptoDataProviderSettingsRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<ExternalCryptoProviderService>>();
            _mockLoggerSettings = new Mock<ILogger<CryptoDataProviderSettingsService>>();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _sut = new ExternalCryptoProviderService(
                _mockHttpClientFactory.Object,
                new CryptoDataProviderSettingsService(_memoryCache, _mockSettingsRepository.Object, _mockConfiguration.Object, _mockLoggerSettings.Object),
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetExternalCryptoDataForCurrencyCodeAsync_Success()
        {
            // Arrange
            var cryptoCurrencyCode = "BTC";
            var expectedResponse = new Dictionary<string, ExternalCryptoDataProviderCryptoQuote>
            {
                {
                    "USD", new ExternalCryptoDataProviderCryptoQuote
                    {
                        Price = 50000.0m,
                        LastUpdated = DateTime.UtcNow
                    }
                }
            };

            var providerSettings = new ExternalCryptoProviderSettings
            {
                BaseUrl = "https://example.com/api",
                RequiredCurrencies = "USD,EUR",
                Headers = "ApiKey", 
                ApiKey = "your-api-key"
            };

            _memoryCache.Set("ExternalCryptoProviderSettings", providerSettings);

            _mockSettingsRepository.Setup(s => s.GetSettingsForDefaultProvider(It.IsAny<string>())).ReturnsAsync(providerSettings);

            _mockHttpClient.Setup(c => c.SendAsync(It.Is<HttpRequestMessage>(x => x.RequestUri.Equals("https://example.com/api/quotes/latest?convert=USD,EUR&symbol=BTC")), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new HttpResponseMessage
                           {
                               StatusCode = HttpStatusCode.OK,
                               ReasonPhrase = "OK",
                           });

            _mockHttpClientFactory.Setup(s => s.CreateClient(string.Empty)).Returns(_mockHttpClient.Object);

            // Act
            var result = await _sut.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrencyCode);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }


    }
}

// _memoryCache.Set("ExternalCryptoProviderSettings", providerSettings);

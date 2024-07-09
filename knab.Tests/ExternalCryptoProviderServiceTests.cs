using FluentAssertions;
using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.DataAccess.Services;
using knab.Shared.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace knab.Shared.Tests
{
    public class ExternalCryptoProviderServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<ICryptoDataProviderSettingsRepository> _mockSettingsRepository;
        private readonly Mock<ICryptoCurrencyPropertyRepository> _mockPropertyRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<ExternalCryptoProviderService>> _mockLogger;
        private readonly Mock<ILogger<CryptoDataProviderSettingsService>> _mockLoggerSettings;
        private readonly Mock<ILogger<CryptoCurrencyPropertyService>> _mockLoggerProperties;

        private readonly MemoryCache _memoryCache;
        private readonly ExternalCryptoProviderService _sut;



        public ExternalCryptoProviderServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClient = new();
            _mockSettingsRepository = new Mock<ICryptoDataProviderSettingsRepository>();
            _mockPropertyRepository = new Mock<ICryptoCurrencyPropertyRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<ExternalCryptoProviderService>>();
            _mockLoggerSettings = new Mock<ILogger<CryptoDataProviderSettingsService>>();
            _mockLoggerProperties = new Mock<ILogger<CryptoCurrencyPropertyService>>();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _sut = new ExternalCryptoProviderService(
                _mockHttpClientFactory.Object,
                new CryptoDataProviderSettingsService(_memoryCache, _mockSettingsRepository.Object, _mockConfiguration.Object, _mockLoggerSettings.Object),
                new CryptoCurrencyPropertyService(_memoryCache, _mockPropertyRepository.Object, _mockLoggerProperties.Object),
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetExternalCryptoDataForCurrencyCodeAsync_Success()
        {
            var cryptoCurrencyCode = "BTC";

            _memoryCache.Set("ExternalCryptoProviderSettings", GetValidSettings());
            _memoryCache.Set("CryptoCurrencyProperties", GetValidProperties());

            _mockSettingsRepository.Setup(s => s.GetSettingsForDefaultProvider(It.IsAny<string>())).ReturnsAsync(GetValidSettings());
            _mockPropertyRepository.Setup(s => s.GetCryptoCurrencyPropertiesAsync()).ReturnsAsync(GetValidProperties());

            _mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new HttpResponseMessage
                           {
                               StatusCode = HttpStatusCode.OK,
                               ReasonPhrase = "OK",
                               Content = new StringContent(GetValidResponseString())
                           });

            _mockHttpClientFactory.Setup(s => s.CreateClient(string.Empty)).Returns(_mockHttpClient.Object);

            var result = await _sut.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrencyCode);

            result.Should().NotBeNull();
            result.Should().ContainKey("EUR");
            result.Should().ContainKey("USD");

            result["EUR"].Price.Should().Be(0.08253728561286233M);
            result["EUR"].LastUpdated.Should().Be(DateTime.Parse("2024-07-09T09:58:15.681Z").ToUniversalTime());

            result["USD"].Price.Should().Be(0.08253728561286233M);
            result["USD"].LastUpdated.Should().Be(DateTime.Parse("2024-07-09T09:58:15.681Z").ToUniversalTime());
        }

        [Fact]
        public async Task GetExternalCryptoDataForCurrencyCodeAsync_Exception_ThrowsException()
        {
            var cryptoCurrencyCode = "BTC";

            _memoryCache.Set("ExternalCryptoProviderSettings", GetValidSettings());
            _memoryCache.Set("CryptoCurrencyProperties", GetValidProperties());

            _mockSettingsRepository.Setup(s => s.GetSettingsForDefaultProvider(It.IsAny<string>())).ReturnsAsync(GetValidSettings());
            _mockPropertyRepository.Setup(s => s.GetCryptoCurrencyPropertiesAsync()).ReturnsAsync(GetValidProperties());

            _mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new Exception());

            _mockHttpClientFactory.Setup(s => s.CreateClient(string.Empty)).Returns(_mockHttpClient.Object);

            await _sut.Invoking(sut => sut.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrencyCode))
                       .Should()
                       .ThrowAsync<Exception>("An unexpected error occurred while fetching external crypto data.");
        }

        [Fact]
        public async Task GetExternalCryptoDataForCurrencyCodeAsync_TaskCanceledException_ThrowsTaskCanceledException()
        {
            var cryptoCurrencyCode = "BTC";

            _memoryCache.Set("ExternalCryptoProviderSettings", GetValidSettings());
            _memoryCache.Set("CryptoCurrencyProperties", GetValidProperties());

            _mockSettingsRepository.Setup(s => s.GetSettingsForDefaultProvider(It.IsAny<string>())).ReturnsAsync(GetValidSettings());
            _mockPropertyRepository.Setup(s => s.GetCryptoCurrencyPropertiesAsync()).ReturnsAsync(GetValidProperties());

            _mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new TaskCanceledException());

            _mockHttpClientFactory.Setup(s => s.CreateClient(string.Empty)).Returns(_mockHttpClient.Object);

            await _sut.Invoking(sut => sut.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrencyCode))
                       .Should()
                       .ThrowAsync<TaskCanceledException>();
        }

        [Fact]
        public async Task GetExternalCryptoDataForCurrencyCodeAsync_HttpRequestException_ThrowsHttpRequestException()
        {
            var cryptoCurrencyCode = "BTC";

            _memoryCache.Set("ExternalCryptoProviderSettings", GetValidSettings());
            _memoryCache.Set("CryptoCurrencyProperties", GetValidProperties());

            _mockSettingsRepository.Setup(s => s.GetSettingsForDefaultProvider(It.IsAny<string>())).ReturnsAsync(GetValidSettings());
            _mockPropertyRepository.Setup(s => s.GetCryptoCurrencyPropertiesAsync()).ReturnsAsync(GetValidProperties());

            _mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new HttpRequestException());

            _mockHttpClientFactory.Setup(s => s.CreateClient(string.Empty)).Returns(_mockHttpClient.Object);

            await _sut.Invoking(sut => sut.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrencyCode))
                       .Should()
                       .ThrowAsync<HttpRequestException>();
        }

        private string GetValidResponseString()
        {
            var jsonResponse = new
            {
                status = new
                {
                    timestamp = "2024-07-09T09:58:15.681Z",
                    error_code = 0,
                    error_message = (string)null,
                    elapsed = 1,
                    credit_count = 5,
                    notice = (string)null
                },
                data = new
                {
                    bitcoin = new
                    {
                        quote = new
                        {
                            EUR = new
                            {
                                price = 0.08253728561286233M,
                                last_updated = "2024-07-09T09:58:15.681Z"
                            },
                            USD = new
                            {
                                price = 0.08253728561286233M,
                                last_updated = "2024-07-09T09:58:15.681Z"
                            }
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(jsonResponse);
        }

        private ExternalCryptoProviderSettings GetValidSettings()
        {
            return new ExternalCryptoProviderSettings
            {
                BaseUrl = "https://example.com/api",
                RequiredCurrencies = "USD,EUR",
                Headers = "ApiKey",
                ApiKey = "your-api-key"
            };
        }

        private List<CryptoCurrencyProperty> GetValidProperties()
        {
            return new List<CryptoCurrencyProperty>
            {
                new CryptoCurrencyProperty {
                        Name ="Bitcoin",
                        Symbol = "BTC",
                        Slug = "bitcoin"
                }
            };
        }

    }
}

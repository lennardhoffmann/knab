using FluentAssertions;
using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.Shared.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

public class CryptoDataProviderSettingsServiceTests
{
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly Mock<ICryptoDataProviderSettingsRepository> _mockSettingsRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<CryptoDataProviderSettingsService>> _mockLogger;

    private readonly CryptoDataProviderSettingsService _sut;

    public CryptoDataProviderSettingsServiceTests()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockSettingsRepository = new Mock<ICryptoDataProviderSettingsRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<CryptoDataProviderSettingsService>>();

        _sut = new CryptoDataProviderSettingsService(
            _mockMemoryCache.Object,
            _mockSettingsRepository.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetCryptoProviderSettingsAsync_FromCache()
    {
        var settings = GetValidSettings();
        _mockMemoryCache.Setup(c => c.TryGetValue("ExternalCryptoProviderSettings", out settings))
                       .Returns(true);

        var result = await _sut.GetCryptoProviderSettingsAsync();

        result.Should().BeSameAs(settings); // Verify that cached settings are returned
    }

    [Fact]
    public async Task GetCryptoProviderSettingsAsync_FromDatabase_Success()
    {
        var settingsFromDb = GetValidSettings();

        _mockMemoryCache.Setup(c => c.TryGetValue("ExternalCryptoProviderSettings", out It.Ref<object>.IsAny))
                       .Returns(false);

        _mockConfiguration.Setup(c => c.GetSection("DefaultExternalCryptoProvider").Value)
                      .Returns("someProvider");

        _mockSettingsRepository.Setup(r => r.GetSettingsForDefaultProvider(It.IsAny<string>()))
                               .ReturnsAsync(settingsFromDb);

        var result = await _sut.GetCryptoProviderSettingsAsync();

        result.Should().BeSameAs(settingsFromDb);

    }

    [Fact]
    public async void GetCryptoProviderSettingsAsync_FromDatabase_Failure()
    {
        // Arrange
        _mockMemoryCache.Setup(c => c.TryGetValue("ExternalCryptoProviderSettings", out It.Ref<object>.IsAny))
                       .Returns(false);

        _mockConfiguration.Setup(c => c.GetSection("DefaultExternalCryptoProvider").Value)
                      .Returns("someProvider");

        _mockSettingsRepository.Setup(r => r.GetSettingsForDefaultProvider("someProvider"))
                               .ReturnsAsync((ExternalCryptoProviderSettings)null);

        await _sut.Invoking(sut => sut.GetCryptoProviderSettingsAsync())
                       .Should()
                       .ThrowAsync<InvalidOperationException>("Settings for the default provider could not be retrieved.");

    }

    [Fact]
    public async void GetCryptoProviderSettingsAsync_Configuration_Missing()
    {
        _mockMemoryCache.Setup(c => c.TryGetValue("ExternalCryptoProviderSettings", out It.Ref<object>.IsAny))
                       .Returns(false); 

        _mockConfiguration.Setup(c => c.GetSection("DefaultExternalCryptoProvider").Value)
                          .Returns((string)null);


        await _sut.Invoking(sut => sut.GetCryptoProviderSettingsAsync())
                      .Should()
                      .ThrowAsync<InvalidOperationException>("Settings for the default provider could not be retrieved.");
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
}

using FluentAssertions;
using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.DataAccess.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

public class CryptoCurrencyPropertyServiceTests
{
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly Mock<ICryptoCurrencyPropertyRepository> _mockPropertyRepository;
    private readonly Mock<ILogger<CryptoCurrencyPropertyService>> _mockLogger;

    private readonly CryptoCurrencyPropertyService _sut;

    public CryptoCurrencyPropertyServiceTests()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockPropertyRepository = new Mock<ICryptoCurrencyPropertyRepository>();
        _mockLogger = new Mock<ILogger<CryptoCurrencyPropertyService>>();

        _sut = new CryptoCurrencyPropertyService(
            _mockMemoryCache.Object,
            _mockPropertyRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetCryptoCurrencyPropertiesAsync_FromCache()
    {
        var properties = GetValidProperties();

        _mockMemoryCache.Setup(c => c.TryGetValue("CryptoCurrencyProperties", out properties))
                       .Returns(true);

        var result = await _sut.GetCryptoCurrencyPropertiesAsync();

        result.Should().BeSameAs(properties);
    }

    [Fact]
    public async void GetCryptoCurrencyPropertiesAsync_FetchFromDatabase_Failure()
    {
        // Arrange
        _mockMemoryCache.Setup(c => c.TryGetValue("CryptoCurrencyProperties", out It.Ref<object>.IsAny))
                       .Returns(false);

        _mockPropertyRepository.Setup(r => r.GetCryptoCurrencyPropertiesAsync())
                                .ReturnsAsync((List<CryptoCurrencyProperty>)null);

        await _sut.Invoking(sut => sut.GetCryptoCurrencyPropertiesAsync())
                      .Should()
                      .ThrowAsync<InvalidOperationException>("Crypto currency properties could not be retrieved.");
    }

    private List<CryptoCurrencyProperty> GetValidProperties()
    {
        return
            [
                new CryptoCurrencyProperty {
                        Name ="Bitcoin",
                        Symbol = "BTC",
                        Slug = "bitcoin"
                }
            ];
    }
}

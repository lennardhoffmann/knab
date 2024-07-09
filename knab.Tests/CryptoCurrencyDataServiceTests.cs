using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.DataAccess.Services;
using knab.Shared.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

public class CryptoCurrencyDataServiceTests
{
    private readonly Mock<ICryptoCurrencyDataRequestRepository> _mockDataRequestRepository;
    private readonly Mock<ILogger<CryptoCurrencyDataService>> _mockLogger;
    private readonly Mock<CryptoCurrencyPropertyService> _mockPropertyService;
    private readonly CryptoCurrencyDataService _service;

    public CryptoCurrencyDataServiceTests()
    {
        _mockDataRequestRepository = new Mock<ICryptoCurrencyDataRequestRepository>();
        _mockLogger = new Mock<ILogger<CryptoCurrencyDataService>>();
        _mockPropertyService = new Mock<CryptoCurrencyPropertyService>(
            MockBehavior.Strict,
            null,
            null, 
            null); 

        _service = new CryptoCurrencyDataService(
            _mockDataRequestRepository.Object,
            _mockLogger.Object,
            _mockPropertyService.Object);
    }

    [Fact]
    public async Task GetCryptoCurrencyProperties_ShouldReturnProperties()
    {
        var expectedProperties = GetValidProperties();
        _mockPropertyService.Setup(s => s.GetCryptoCurrencyPropertiesAsync())
                            .ReturnsAsync(expectedProperties);

        var result = await _service.GetCryptoCurrencyProperties();

        result.Should().BeEquivalentTo(expectedProperties);
    }

    [Fact]
    public async Task StoreRequestForCryptoCurrency_NewRecord_ShouldAddRecord()
    {
        // Arrange
        var cryptoCurrencyCode = "BTC";
        var externalCryptoResponse = new Dictionary<string, ExternalCryptoDataProviderCryptoQuote>
        {
            { "EUR", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } },
            { "USD", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } }
        };

        _mockDataRequestRepository.Setup(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode))
                                  .ReturnsAsync((CryptoCurrencyDataRequest)null); // Simulate record not found

        _mockDataRequestRepository.Setup(r => r.AddDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()))
                                  .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _service.StoreRequestForCryptoCurrency(cryptoCurrencyCode, externalCryptoResponse);

        // Assert
        await act.Should().NotThrowAsync();
        _mockDataRequestRepository.Verify(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode), Times.Once);
        _mockDataRequestRepository.Verify(r => r.AddDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Once);
        _mockDataRequestRepository.Verify(r => r.UpdateDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Never);
    }

    [Fact]
    public async Task StoreRequestForCryptoCurrency_ExistingRecord_ShouldUpdateRecord()
    {
        // Arrange
        var cryptoCurrencyCode = "BTC";
        var externalCryptoResponse = new Dictionary<string, ExternalCryptoDataProviderCryptoQuote>
        {
            { "EUR", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } },
            { "USD", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } }
        };

        var existingRecord = new CryptoCurrencyDataRequest { CurrencyCode = cryptoCurrencyCode, History = new List<CryptoCurrencyDataRequestHistory>() };

        _mockDataRequestRepository.Setup(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode))
                                  .ReturnsAsync(existingRecord);

        _mockDataRequestRepository.Setup(r => r.UpdateDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()))
                                  .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _service.StoreRequestForCryptoCurrency(cryptoCurrencyCode, externalCryptoResponse);

        // Assert
        await act.Should().NotThrowAsync();
        _mockDataRequestRepository.Verify(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode), Times.Once);
        _mockDataRequestRepository.Verify(r => r.AddDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Never);
        _mockDataRequestRepository.Verify(r => r.UpdateDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Once);
    }

    [Fact]
    public async Task StoreRequestForCryptoCurrency_TransientError_Retries()
    {
        // Arrange
        var cryptoCurrencyCode = "BTC";
        var externalCryptoResponse = new Dictionary<string, ExternalCryptoDataProviderCryptoQuote>
        {
            { "EUR", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } },
            { "USD", new ExternalCryptoDataProviderCryptoQuote { Price = 0.08253728561286233M } }
        };

        _mockDataRequestRepository.SetupSequence(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode))
                                  .ThrowsAsync(new HttpRequestException("Simulated transient error"))
                                  .ReturnsAsync((CryptoCurrencyDataRequest)null);

        _mockDataRequestRepository.Setup(r => r.AddDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()))
                                  .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _service.StoreRequestForCryptoCurrency(cryptoCurrencyCode, externalCryptoResponse);

        // Assert
        await act.Should().NotThrowAsync();
        _mockDataRequestRepository.Verify(r => r.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode), Times.Exactly(2));
        _mockDataRequestRepository.Verify(r => r.AddDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Once);
        _mockDataRequestRepository.Verify(r => r.UpdateDataRequestForCryptoCurrencyAsync(It.IsAny<CryptoCurrencyDataRequest>()), Times.Never);
    }

    private bool IsTransientException(Exception ex)
    {
        // Implement transient exception check logic as needed
        return ex is HttpRequestException || ex is MongoException;
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

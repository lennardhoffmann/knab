using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.Shared.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace knab.DataAccess.Services
{
    public class CryptoCurrencyDataService : ICryptoCurrencyDataService
    {
        private readonly ICryptoCurrencyDataRequestRepository _dataRequestRepository;
        private const int MaxRetryAttempts = 3;
        private readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);
        private readonly ILogger<CryptoCurrencyDataService> _logger;
        private readonly CryptoCurrencyPropertyService _propertyService;

        public CryptoCurrencyDataService(
            ICryptoCurrencyDataRequestRepository dataRequestRepository,
            ILogger<CryptoCurrencyDataService> logger,
           CryptoCurrencyPropertyService propertyService)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger;
            _propertyService = propertyService;
        }
        public async Task<List<CryptoCurrencyProperty>> GetCryptoCurrencyProperties()
        {
            var currencyProperties = await _propertyService.GetCryptoCurrencyPropertiesAsync();

            return currencyProperties;
        }

        public async Task StoreRequestForCryptoCurrency(string cryptoCurrencyCode, Dictionary<string, ExternalCryptoDataProviderCryptoQuote> externalCryptoResponse)
        {
            int attempt = 0;
            bool success = false;

            while (!success && attempt < MaxRetryAttempts)
            {
                attempt++;
                try
                {
                    var requestRecord = await _dataRequestRepository.GetDataRequestRecordByCryptoCurrencyCodeAsync(cryptoCurrencyCode);
                    if (requestRecord == null)
                    {
                        var record = BuildNewRequestRecord(cryptoCurrencyCode, externalCryptoResponse);
                        await _dataRequestRepository.AddDataRequestForCryptoCurrencyAsync(record);
                    }
                    else
                    {
                        requestRecord.History.Add(new CryptoCurrencyDataRequestHistory
                        {
                            SearchDate = DateTime.Now,
                            HistoricSearchData = MapToExternalDictionaryToDataRequestDictionary(externalCryptoResponse)
                        });

                        await _dataRequestRepository.UpdateDataRequestForCryptoCurrencyAsync(requestRecord);
                    }

                    success = true;
                }
                catch (Exception ex) when (IsTransientException(ex))
                {
                    _logger.LogError(ex, $"Transient error occurred while storing request for crypto currency {cryptoCurrencyCode}");

                    if (attempt < MaxRetryAttempts)
                        await Task.Delay(RetryDelay * attempt);
                }
            }

            if (!success)
            {
                _logger.LogError($"Failed to store request for crypto currency {cryptoCurrencyCode} after {MaxRetryAttempts} attempts.");
                throw new ApplicationException($"Failed to store request for crypto currency {cryptoCurrencyCode} after {MaxRetryAttempts} attempts.");
            }
        }

        private bool IsTransientException(Exception ex)
        {
            // Check if the exception is transient and eligible for retry
            return ex is HttpRequestException || ex is MongoException;
        }

        private CryptoCurrencyDataRequest BuildNewRequestRecord(string cryptoCurrencyCode, Dictionary<string, ExternalCryptoDataProviderCryptoQuote> externalCryptoResponse)
        {
            return new CryptoCurrencyDataRequest
            {
                CurrencyCode = cryptoCurrencyCode,
                History =
                [
                    new() {
                        SearchDate = DateTime.Now,
                        HistoricSearchData = MapToExternalDictionaryToDataRequestDictionary(externalCryptoResponse)
                    }
                ]
            };
        }

        private Dictionary<string, CryptoCurrencyDataQuote> MapToExternalDictionaryToDataRequestDictionary(Dictionary<string, ExternalCryptoDataProviderCryptoQuote> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var target = new Dictionary<string, CryptoCurrencyDataQuote>();

            foreach (var kvp in source)
            {
                var currencyCode = kvp.Key;
                var quote = kvp.Value;

                var cryptoCurrency = new CryptoCurrencyDataQuote
                {
                    Code = currencyCode,
                    Price = quote.Price
                };

                target[currencyCode] = cryptoCurrency;
            }

            return target;
        }
    }
}

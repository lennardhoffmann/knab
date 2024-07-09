using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using knab.ExternalCryptoDataProvider.Models;
using MongoDB.Driver;
using System.Text.Json;

namespace knab.API.Services
{
    public class CryptoCurrencyDataService : ICryptoCurrencyDataService
    {
        private readonly ICryptoCurrencyDataRequestRepository _dataRequestRepository;
        private const int MaxRetryAttempts = 3;
        private readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);
        private readonly ILogger<CryptoCurrencyDataService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICryptoCurrencyPropertyRepository _cryptoCurrencyPropertyRepository;

        public CryptoCurrencyDataService(ICryptoCurrencyDataRequestRepository dataRequestRepository, ILogger<CryptoCurrencyDataService> logger, IHttpClientFactory httpClientFactory, ICryptoCurrencyPropertyRepository cryptoCurrencyPropertyRepository)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cryptoCurrencyPropertyRepository = cryptoCurrencyPropertyRepository;
        }
        public async Task GetCryptoCurrencyProperties()
        {
            var client = _httpClientFactory.CreateClient();

            var baseAddress = new Uri($"https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");
            var request = new HttpRequestMessage(HttpMethod.Get, baseAddress);


            request.Headers.Add("X-CMC_PRO_API_KEY", "eaf04d66-cd6a-4574-bd45-a129a9c9605b");


            var response = await client.SendAsync(request, cancellationToken: default);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var cryptoResponse = JsonSerializer.Deserialize<CryptoResponse>(data, options);

            List<CryptoCurrencyPropertyItem> cryptoList = cryptoResponse.Data;

            foreach (var cryptoCurrencyProperty in cryptoList)
            {
                var prop = new CryptoCurrencyProperty
                {
                    Name = cryptoCurrencyProperty.Name,
                    Symbol = cryptoCurrencyProperty.Symbol,
                    Slug = cryptoCurrencyProperty.Slug
                };

                await _cryptoCurrencyPropertyRepository.AddCryptoCurrencypropertyAsync(prop);
            }

            return;
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

using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace knab.DataAccess.Services
{
    public class CryptoCurrencyPropertyService
    {
        private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);
        private readonly IMemoryCache _memoryCache;
        private readonly ICryptoCurrencyPropertyRepository _cryptoCurrencyPropertyRepository;
        private readonly ILogger<CryptoCurrencyPropertyService> _logger;

        public CryptoCurrencyPropertyService(
            IMemoryCache memoryCache,
            ICryptoCurrencyPropertyRepository cryptoCurrencyPropertyRepository,
            ILogger<CryptoCurrencyPropertyService> logger)
        {
            _memoryCache = memoryCache;
            _cryptoCurrencyPropertyRepository = cryptoCurrencyPropertyRepository;
            _logger = logger;
        }

        public async Task<List<CryptoCurrencyProperty>> GetCryptoCurrencyPropertiesAsync()
        {
            if (!_memoryCache.TryGetValue("CryptoCurrencyProperties", out List<CryptoCurrencyProperty> currencyProperties))
            {
                try
                {
                    currencyProperties = await FetchPropertiesFromDatabase();
                    if (currencyProperties == null)
                    {
                        throw new InvalidOperationException("No properties retrieved from the database");
                    }

                    _memoryCache.Set("CryptoCurrencyProperties", currencyProperties, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheDuration
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch properties from the database.");
                    throw;
                }
            }

            return currencyProperties;
        }

        private async Task<List<CryptoCurrencyProperty>> FetchPropertiesFromDatabase()
        {
            var result = await _cryptoCurrencyPropertyRepository.GetCryptoCurrencyPropertiesAsync();

            return result ?? throw new InvalidOperationException("Crypto currency properties could not be retrieved.");
        }
    }
}

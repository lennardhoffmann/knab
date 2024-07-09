using knab.DataAccess.Models;
using knab.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace knab.Shared.Services
{
    public class CryptoDataProviderSettingsService
    {
        private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(1);
        private readonly IMemoryCache _memoryCache;
        private readonly ICryptoDataProviderSettingsRepository _cryptoDataProviderSettingsRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CryptoDataProviderSettingsService> _logger;

        public CryptoDataProviderSettingsService(
            IMemoryCache memoryCache,
            ICryptoDataProviderSettingsRepository cryptoDataProviderSettingsRepository,
            IConfiguration configuration,
            ILogger<CryptoDataProviderSettingsService> logger)
        {
            _memoryCache = memoryCache;
            _cryptoDataProviderSettingsRepository = cryptoDataProviderSettingsRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExternalCryptoProviderSettings> GetCryptoProviderSettingsAsync()
        {
            if (!_memoryCache.TryGetValue("ExternalCryptoProviderSettings", out ExternalCryptoProviderSettings settings))
            {
                try
                {
                    settings = await FetchSettingsFromDatabase();
                    if (settings == null)
                    {
                        throw new InvalidOperationException("No settings retrieved from the database");
                    }

                    _memoryCache.Set("ExternalCryptoProviderSettings", settings, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheDuration
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch settings from the database.");
                    throw;
                }
            }

            return settings;
        }

        private async Task<ExternalCryptoProviderSettings> FetchSettingsFromDatabase()
        {
            var defaultProvider = _configuration.GetSection("DefaultExternalCryptoProvider").Value;
            if (string.IsNullOrEmpty(defaultProvider))
            {
                throw new InvalidOperationException("DefaultExternalCryptoProvider configuration value is missing or empty.");
            }

            var result = await _cryptoDataProviderSettingsRepository.GetSettingsForDefaultProvider(defaultProvider);
            return result == null ? throw new InvalidOperationException("Settings for the default provider could not be retrieved.") : result;
        }
    }
}

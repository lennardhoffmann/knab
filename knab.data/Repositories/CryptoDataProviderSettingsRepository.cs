
using knab.DataAccess.Models;
using MongoDB.Driver;

namespace knab.DataAccess.Repositories
{
    public class CryptoDataProviderSettingsRepository : ICryptoDataProviderSettingsRepository
    {
        private readonly IMongoCollection<ExternalCryptoProviderSettings> _collection;

        public CryptoDataProviderSettingsRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<ExternalCryptoProviderSettings>(nameof(ExternalCryptoProviderSettings));
        }

        public async Task<ExternalCryptoProviderSettings> GetSettingsForDefaultProvider(string defaultProvider)
        {
            var settings = await _collection.Find(x => x.Name == defaultProvider).FirstOrDefaultAsync();

            return settings;
        }
    }
}

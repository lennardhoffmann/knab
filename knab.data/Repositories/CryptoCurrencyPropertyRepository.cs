using knab.DataAccess.Models;
using MongoDB.Driver;

namespace knab.DataAccess.Repositories
{
    public class CryptoCurrencyPropertyRepository : ICryptoCurrencyPropertyRepository
    {
        private readonly IMongoCollection<CryptoCurrencyProperty> _collection;

        public CryptoCurrencyPropertyRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<CryptoCurrencyProperty>(nameof(CryptoCurrencyProperty));
        }

        public async Task AddCryptoCurrencypropertyAsync(CryptoCurrencyProperty currencyproperty)
        {
            await _collection.InsertOneAsync(currencyproperty);
            return;
        }

        public async Task<List<CryptoCurrencyProperty>> GetCryptoCurrencyPropertiesAsync()
        {
            var result = await _collection.Find(Builders<CryptoCurrencyProperty>.Filter.Empty).ToListAsync();
            return result;
        }
    }
}

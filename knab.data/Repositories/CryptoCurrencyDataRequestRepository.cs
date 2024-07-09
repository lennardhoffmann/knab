using knab.DataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace knab.DataAccess.Repositories
{
    public class CryptoCurrencyDataRequestRepository : ICryptoCurrencyDataRequestRepository
    {
        private readonly IMongoCollection<CryptoCurrencyDataRequest> _collection;

        public CryptoCurrencyDataRequestRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<CryptoCurrencyDataRequest>(nameof(CryptoCurrencyDataRequest));
        }
        public async Task<CryptoCurrencyDataRequest> GetDataRequestRecordByCryptoCurrencyCodeAsync(string cryptoCurrencyCode)
        {
            var result = await _collection.Find(x => x.CurrencyCode == cryptoCurrencyCode).FirstOrDefaultAsync();
            return result;
        }

        public async Task AddDataRequestForCryptoCurrencyAsync(CryptoCurrencyDataRequest requestObject)
        {
            await _collection.InsertOneAsync(requestObject);
        }

        public async Task UpdateDataRequestForCryptoCurrencyAsync(CryptoCurrencyDataRequest requestObject)
        {
            var filter = Builders<CryptoCurrencyDataRequest>.Filter.Eq("CurrencyCode", requestObject.CurrencyCode);
            var update = Builders<CryptoCurrencyDataRequest>.Update
           .Set(record => record.History, requestObject.History);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}

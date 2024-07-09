using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace knab.DataAccess.Models
{
    public class CryptoCurrencyDataRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CurrencyCode { get; set; }
        public List<CryptoCurrencyDataRequestHistory> History { get; set; }
    }

    public class CryptoCurrencyDataRequestHistory
    {
        public DateTime SearchDate { get; set; }
        public Dictionary<string, CryptoCurrencyDataQuote> HistoricSearchData { get; set; }

    }
}

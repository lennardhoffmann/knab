using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace knab.DataAccess.Models
{
    public class CryptoCurrencyProperty
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
    }
}

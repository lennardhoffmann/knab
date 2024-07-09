using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace knab.DataAccess.Models
{
    public class ExternalCryptoProviderSettings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string? Headers { get; set; }
        public string RequiredCurrencies { get; set; }
    }
}

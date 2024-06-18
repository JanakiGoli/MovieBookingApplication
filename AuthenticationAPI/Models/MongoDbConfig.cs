using AccessIdentityAPI.Interfaces;

namespace AccessIdentityAPI.Models
{
    public class MongoDbConfig : IMongoDbConfig
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string UserCollectionName { get; set; } = string.Empty;
    }
}

using Cinema.Web.Modules.Catalog.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Cinema.Web.Modules.Catalog.Entities;

namespace Cinema.Web.Modules.Catalog.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = configuration.GetSection("MongoDB")["DatabaseName"];
            
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<MovieReview> MovieReviews => _database.GetCollection<MovieReview>("MovieReviews");
    }
}



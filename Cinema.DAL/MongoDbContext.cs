using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Cinema.DAL.Models.Mongo;

namespace Cinema.DAL
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

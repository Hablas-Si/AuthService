using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Repositories
{
    public class MongoDBLoginRepository : IMongoDBRepository
    {
        private readonly IMongoCollection<LoginModel> LoginUsersCollection;

        public MongoDBLoginRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // trækker connection string og database navn og collectionname fra program.cs aka fra terminalen ved export. Dette er en constructor injection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            LoginUsersCollection = database.GetCollection<LoginModel>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<bool> CheckIfUserExists(string Username)
        {
            // Finder en bruger med det indtastede brugernavn ved at lave en midligertidig instans af LoginModel (new BsonDocument("Username", Username) og derefter finde den første bruger med det brugernavn.
            var user = await LoginUsersCollection.Find(new BsonDocument("Username", Username)).FirstOrDefaultAsync();
            // Hvis brugeren findes returneres true, ellers false.
            return user != null;
        }
        public async Task<bool> CheckIfUserExistsWithPassword(string Username, string Password, string Role)
        {
            // Bruger find for at finde en bruger med det indtastede brugernavn og password og role. Hvis brugeren findes returneres den ellers null.
            var user = await LoginUsersCollection.Find(new BsonDocument("Username", Username).Add("Password", Password).Add("Role", Role)).FirstOrDefaultAsync();
            // Hvis brugeren findes returneres true, ellers false.
            return user != null;
        }
    }
}
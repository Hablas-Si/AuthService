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

        public async Task<LoginModel> GetLoginUser(string Username)
        {
            return await LoginUsersCollection.Find(new BsonDocument("Username", Username)).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfUserExists(string Username)
        {
            var user = await LoginUsersCollection.Find(new BsonDocument("Username", Username)).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task AddLoginUser(LoginModel login)
        {
            await LoginUsersCollection.InsertOneAsync(login);
        }

        public async Task UpdateLoginUser(LoginModel login)
        {
            await LoginUsersCollection.ReplaceOneAsync(new BsonDocument("Username", login.Username), login);
        }

        public async Task DeleteLoginUser(string Username)
        {
            await LoginUsersCollection.DeleteOneAsync(new BsonDocument("Username", Username));
        }

    }
}
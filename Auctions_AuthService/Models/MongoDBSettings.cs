namespace Models
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = "AuthDB";
        public string CollectionName { get; set; } = "LoginUsersCollection";
    }
}
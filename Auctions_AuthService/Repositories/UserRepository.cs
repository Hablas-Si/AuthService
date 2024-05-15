using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly HttpClient _httpClient;

        public UserRespository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetUserAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/User/getuser/{userId}");
            response.EnsureSuccessStatusCode();
            return response;
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
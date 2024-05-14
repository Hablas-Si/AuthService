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
    }

}
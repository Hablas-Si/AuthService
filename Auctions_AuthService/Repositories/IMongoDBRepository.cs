using System;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IMongoDBRepository
    {
        Task<bool> CheckIfUserExists(string Username);
        Task<bool> CheckIfUserExistsWithPassword(string Username, string Password, string role);
        Task AddLoginUser(LoginModel login);
        Task<LoginModel> FindUser(Guid id);
        Task UpdateUser(LoginModel login);
        Task DeleteUser(Guid id);
    }
}
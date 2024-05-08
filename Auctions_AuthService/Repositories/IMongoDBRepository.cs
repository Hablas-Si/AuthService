using System;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IMongoDBRepository
    {
        Task<LoginModel> GetLoginUser(string Username);
        Task<bool> CheckIfUserExists(string Username);
        Task AddLoginUser(LoginModel login);
        Task UpdateLoginUser(LoginModel login);
        Task DeleteLoginUser(string Username);
    }
}
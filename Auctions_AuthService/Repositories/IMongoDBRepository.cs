using System;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IMongoDBRepository
    {
        Task<bool> CheckIfUserExistsWithPassword(string Username, string Password, string role);
    }
}
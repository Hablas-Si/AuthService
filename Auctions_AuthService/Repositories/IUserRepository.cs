using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<HttpResponseMessage> GetUserAsync(Guid userId);
        Task<bool> ValidateUserAsync(LoginModel login);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepository
    {
        public Task<HttpResponseMessage> GetUserAsync(Guid userId);

    }
}
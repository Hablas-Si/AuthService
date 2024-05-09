using System;
using System.Collections.Generic;
using Models;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IVaultService
    {
        Task<string> GetSecret(string path, int version, string mountPoint);
    }
}
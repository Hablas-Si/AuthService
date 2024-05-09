using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;

namespace Repositories
{
    public class VaultService : IVaultService
    {
        private readonly IVaultClient _vaultClient;

        public VaultService(string token, string endPoint)
        {
            var authMethod = new TokenAuthMethodInfo(token);
            var vaultClientSettings = new VaultClientSettings(endPoint, authMethod);
            _vaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<string> GetSecret(string path, int version, string mountPoint)
        {
            var kv2Secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, version, mountPoint);
            return kv2Secret.Data.Data["ConnectionURI"].ToString();
        }

    }
}
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Models;
using VaultSharp.V1.AuthMethods;

namespace Repositories
{
    public class VaultService : IVaultService
    {
        private readonly IVaultClient _vaultClient;

        public VaultService()
        {
            var EndPoint = Environment.GetEnvironmentVariable("Address");
            var token = Environment.GetEnvironmentVariable("Token");

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, sslPolicyErrors) => { return true; };
            IAuthMethodInfo authMethod =
                new TokenAuthMethodInfo(token);
            var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
            {
                Namespace = "",
                MyHttpClientProviderFunc = handler
                    => new HttpClient(httpClientHandler)
                    {
                        BaseAddress = new Uri(EndPoint)
                    }
            };
            _vaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<string> GetSecretAsync(string path)
        {
            try
            {

                Secret<SecretData> kv2Secret = await _vaultClient.V1.Secrets.KeyValue.V2
                    .ReadSecretAsync(path: "hemmeligheder", mountPoint: "secret");
                var secretValue = kv2Secret.Data.Data[path];
                return secretValue.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving secret: {ex.Message}");
                return null;
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;
using WebApplication1.Pojos;

namespace WebApplication1.Services
{
    public class VaultClientService
    {
        private VaultClient vaultClient;

        public VaultClientService()
        {
            var RoleID = "e95aff2d-f892-a8c4-52c6-f0c00ea44057";
            var SecretID = "fb198d96-e259-e84c-46d6-4dd0379d206e";

            IAuthMethodInfo authMethod2 = new AppRoleAuthMethodInfo(RoleID, SecretID);

            var vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod2);

            vaultClient = new VaultClient(vaultClientSettings);

            //GetCredentials();               

        }

        public async Task<Credentials> GetCredentials()
        {
            Secret<UsernamePasswordCredentials> secret = await vaultClient.V1.Secrets.RabbitMQ.GetCredentialsAsync("bridge");
            string username = secret.Data.Username;
            string password = secret.Data.Password;

            Console.WriteLine($"GET New RabbiMQ Creds username: {username} password: {password}");

            return new Credentials { UserName = username, Password = password };
        }
    }
}

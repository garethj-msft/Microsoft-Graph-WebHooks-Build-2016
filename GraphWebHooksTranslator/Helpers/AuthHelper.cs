using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace UnifiedApiConnect.Helpers
{
    public static class AuthHelper
    {
        public static async Task<string> RetrieveAccessTokenAsync(string refreshToken)
        {
            var authContext = new AuthenticationContext(Settings.AzureADAuthority);

            AuthenticationResult result =
                await authContext.AcquireTokenByRefreshTokenAsync(refreshToken,
                new ClientCredential(Settings.ClientId, Settings.ClientSecret), // use the client ID and secret to establish app identity.
                Settings.MicrosoftGraphResource);

            return result?.AccessToken;
        }
    }
}
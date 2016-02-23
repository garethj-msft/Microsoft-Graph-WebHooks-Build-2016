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
                await authContext.AcquireTokenByRefreshTokenAsync(refreshToken, Settings.ClientId);
            return result?.AccessToken;
        }
    }
}
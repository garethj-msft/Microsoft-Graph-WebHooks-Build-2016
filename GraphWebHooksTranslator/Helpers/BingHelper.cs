using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ValueProviders.Providers;
using System.Xml.Linq;
using Newtonsoft.Json;
using GraphWebhooksTranslator.Models;

namespace GraphWebhooksTranslator.Helpers
{
    public class BingHelper
    {
        public static async Task<string> TranslateAsync(string subject, string language)
        {
            AdmAccessToken admToken;

            // Get Bing Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            AdmAuthentication admAuth = new AdmAuthentication(Settings.ClientId, Settings.TranslatorSecret);
            try
            {
                admToken = await admAuth.GetAccessToken();
                string uri = $"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={System.Web.HttpUtility.UrlEncode(subject)}&from=en&to={language.ToLowerInvariant()}";
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admToken.access_token);
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                                {
                                    XElement translation = XElement.Load(responseStream);
                                    return translation.Value;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private readonly string clientId;

        private readonly string requestParams;
        private AdmAccessToken token = null;

        
        private Lazy<Timer> accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;

            //If clientid or client secret has special characters, encode before sending request
            this.requestParams = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));

           //renew the token every specfied minutes
            accessTokenRenewer =
                new Lazy<Timer>(
                    () =>
                        new Timer(new TimerCallback(OnTokenExpiredCallback), this,
                            TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1)));
        }

        public async Task<AdmAccessToken> GetAccessToken()
        {
            if (this.token == null)
            {
                this.token = await HttpPost();

                // Ensure there is a timer
                var renewer = accessTokenRenewer.Value;
            }
            return this.token;
        }

        private async Task RenewAccessToken()
        {
            AdmAccessToken newAccessToken = await HttpPost();
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token);
        }

        private async void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                await RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed renewing access token. Details: {0}", ex.Message);
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Value.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message);
                }
            }
        }
        private async Task<AdmAccessToken> HttpPost()
        {
            //Prepare OAuth request 
            AdmAccessToken token = null;
          
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, DatamarketAccessUri))
                {
                    request.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(this.requestParams));
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            token = JsonConvert.DeserializeObject<AdmAccessToken>(responseString);
                            return token;
                        }

                    }
                }
            }
            return null;
        }
    }
}
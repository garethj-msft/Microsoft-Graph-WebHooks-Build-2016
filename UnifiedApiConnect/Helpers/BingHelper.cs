using System;
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
using System.Xml.Linq;
using Newtonsoft.Json;
using UnifiedApiConnect.Models;

namespace UnifiedApiConnect.Helpers
{
    public class BingHelper
    {
        public static async Task<string> TranslateAsync(string subject, Language language)
        {
            AdmAccessToken admToken;
            string headerValue;

            //Get Bing Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            AdmAuthentication admAuth = new AdmAuthentication(Settings.ClientId, Settings.TranslatorSecret);
            try
            {
                admToken = admAuth.GetAccessToken();
                string uri = $"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={System.Web.HttpUtility.UrlEncode(subject)}&from=en&to={language.ToString().ToLowerInvariant()}";
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
            catch (Exception e)
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
        private string clientId;
        private string clientSecret;
        private string request;
        private AdmAccessToken token;
        private Timer accessTokenRenewer;
        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;
        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
            this.token = HttpPost(DatamarketAccessUri, this.request);
            //renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback), this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
        }
        public AdmAccessToken GetAccessToken()
        {
            return this.token;
        }
        private void RenewAccessToken()
        {
            AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token));
        }
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }
        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
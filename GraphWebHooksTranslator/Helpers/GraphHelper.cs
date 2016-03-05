// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnifiedApiConnect.Models;

namespace UnifiedApiConnect.Helpers
{
    public class GraphHelper
    {
        private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");

        /// <summary>
        /// Create a subscription on the Microsoft Graph
        /// </summary>
        /// <returns>A union of a created subscription or an error</returns>
        public static async Task<SubscriptionResponse> CreateSubscriptionAsync(string accessToken, Subscription subscription)
        {
            var subscriptionResponse = new SubscriptionResponse();
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, Settings.CreateSubscriptionUrl))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    string packetContent = JsonConvert.SerializeObject(subscription,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    request.Content = new StringContent(packetContent, Encoding.UTF8, "application/json");
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            subscription = JsonConvert.DeserializeObject<Subscription>(responseString);
                            subscriptionResponse.Subscription = subscription;
                        }
                        else
                        {
                            subscriptionResponse.Error = responseString;
                        }
                    }
                }
            }
            return subscriptionResponse;
        }

        public static async Task<string> GetEventSubjectAsync(string accessToken, string eventId)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://graph.microsoft.com/beta/{eventId}?$select=subject"))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            var thingWithSubject = new {subject = ""};
                            thingWithSubject = Cast(thingWithSubject, JsonConvert.DeserializeObject(responseString, thingWithSubject.GetType()));
                            return thingWithSubject.subject;
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<bool> SetEventSubjectAsync(string accessToken, string eventId, string subject)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(PatchMethod, $"https://graph.microsoft.com/beta/{eventId}"))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var thingWithSubject = new { subject = subject };
                    string packetContent = JsonConvert.SerializeObject(thingWithSubject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    request.Content = new StringContent(packetContent, Encoding.UTF8, "application/json");
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        return response.IsSuccessStatusCode;
                    }
                }
            }
        }

        private static T Cast<T>(T typeHolder, object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }

    }
}

//********************************************************* 
// 
//O365-AspNetMVC-Unified-API-Connect, https://github.com/OfficeDev/O365-AspNetMVC-Unified-API-Connect
//
//Copyright (c) Microsoft Corporation
//All rights reserved. 
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:

// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
//********************************************************* 

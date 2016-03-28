// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GraphWebhooksTranslator.Models;
using Microsoft.Graph;
using Newtonsoft.Json.Converters;

namespace GraphWebhooksTranslator.Helpers
{
    public class GraphHelper
    {
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
                    string packetContent = JsonConvert.SerializeObject(subscription, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new List<JsonConverter> {new StringEnumConverter {CamelCaseText = true}}
                    });
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

        public static async Task<string> GetEventSubjectAsync(string accessToken, string eventPath)
        {
            using (var graphClient = CreateGraphClient(accessToken))
            {
                var request = new EventRequest(graphClient.BaseUrl + "/" + eventPath, graphClient, null);
                try
                {
                    Event calendarEvent = await request.GetAsync();
                    return calendarEvent.Subject;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static async Task<bool> SetEventSubjectAsync(string accessToken, string eventPath, string subject)
        {
            using (var graphClient = CreateGraphClient(accessToken))
            {
                var request = new EventRequest(graphClient.BaseUrl + "/" + eventPath, graphClient, null);
                try
                {
                    Event updated = await request.UpdateAsync(new Event { Subject = subject });
                    return updated.Subject == subject;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static GraphServiceClient CreateGraphClient(string accessToken)
        {
            return new GraphServiceClient(new DelegateAuthenticationProvider(
                requestMessage =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                    return Task.FromResult(0);
                }));
        }
    }
}

//********************************************************* 
// 
//https://github.com/microsoftgraph/sample-aspnetmvc-webhookstranslator/
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

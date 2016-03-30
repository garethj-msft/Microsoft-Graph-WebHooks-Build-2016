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
        public static async Task<string> GetEventSubjectAsync(string accessToken, string eventPath)
        {
            var graphClient = CreateGraphClient(accessToken);
            var request = new EventRequest(graphClient.BaseUrl + "/" + eventPath, graphClient, null);
            try
            {
                Event calendarEvent = await request.GetAsync();
                return calendarEvent.Subject;
            }
            catch (ServiceException)
            {
                return null;
            }
        }

        public static async Task<bool> SetEventSubjectAsync(string accessToken, string eventPath, string subject)
        {
            var graphClient = CreateGraphClient(accessToken);
            var request = new EventRequest(graphClient.BaseUrl + "/" + eventPath, graphClient, null);
            try
            {
                Event updated = await request.UpdateAsync(new Event { Subject = subject });
                return updated.Subject == subject;
            }
            catch (ServiceException)
            {
                return false;
            }
        }

        public static GraphServiceClient CreateGraphClient(string accessToken)
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

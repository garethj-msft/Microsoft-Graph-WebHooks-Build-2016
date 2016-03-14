using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GraphWebhooksTranslator.Helpers;
using GraphWebhooksTranslator.Models;

namespace GraphWebhooksTranslator.Controllers
{
    public class NotificationsController : ApiController
    {
        /// <summary>
        /// POST: api/Notification
        /// Handle validation
        /// </summary>
        /// <param name="validationToken"></param>
        public HttpResponseMessage Post([FromUri]string validationToken)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            // Important to set Content as a property initialization to trigger content negotiation.
            response.Content = new StringContent(validationToken, Encoding.UTF8, "text/plain");
            return response;
        }

        /// <summary>
        /// POST: api/Notification
        /// Handle notification messages
        /// </summary>
        /// <param name="payload"></param>
        public async Task<HttpResponseMessage> Post([FromBody]Notifications payload)
        {
            const string separator = @"  ---  ";

            try
            {
                // Get the title of the first event, translate it and write it back.
                // In a real app, this would be offloaded onto a more synchronous process in order to handle load smoothly.
                Notification notification = payload.Value.FirstOrDefault();
                if (notification != null)
                {
                    if (notification.ClientState == Settings.VerificationToken)
                    {
                        UserInfoEntity userInfo = await UserDataTable.RetrieveUserInfo(notification.SubscriptionId);
                        if (userInfo != null)
                        {
                            string accessToken = await AuthHelper.RetrieveAccessTokenAsync(userInfo.RefreshToken);
                            if (accessToken != null)
                            {
                                string subject =
                                    await GraphHelper.GetEventSubjectAsync(accessToken, notification.Resource);
                                if (subject != null && !subject.Contains(separator))
                                {
                                    string translated = await BingHelper.TranslateAsync(subject, userInfo.LanguageString);
                                    if (translated != null)
                                    {
                                        subject = subject + separator + translated;
                                        await
                                            GraphHelper.SetEventSubjectAsync(accessToken, notification.Resource, subject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Just return - notifications handlers need to be robust or they will be turned off
            }
            return Request.CreateResponse(HttpStatusCode.OK);
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


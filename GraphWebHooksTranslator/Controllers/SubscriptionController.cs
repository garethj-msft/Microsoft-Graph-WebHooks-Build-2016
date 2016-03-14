// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using GraphWebhooksTranslator.Helpers;
using GraphWebhooksTranslator.Models;

namespace GraphWebhooksTranslator.Controllers
{
    // Manage subscriptions.
    public class SubscriptionController : Controller
    {
        public ActionResult Index(UserInfoEntity userInfo)
        {
            RestoreUserFromSession(ref userInfo);

            ViewBag.UserInfo = userInfo;
            ViewBag.IsLoggedIn = !string.IsNullOrWhiteSpace(Session[SessionKeys.Login.AccessToken].ToString());
            return View();
        }

        public ActionResult Result(UserInfoEntity userInfo)
        {
            RestoreUserFromSession(ref userInfo);

            ViewBag.UserInfo = userInfo;
            ViewBag.IsLoggedIn = !string.IsNullOrWhiteSpace(Session[SessionKeys.Login.AccessToken].ToString());
            return View();
        }

        public async Task<ActionResult> CreateSubscription(UserInfoEntity userInfo)
        {
            // Get the posted Language from the query parameters into the userInfo.
            Language lang = userInfo.Language;
            RestoreUserFromSession(ref userInfo);
            userInfo.Language = lang;
            string accessToken = (string)Session[SessionKeys.Login.AccessToken];

            // Create a subscription on the Microsoft Graph
            var subscription = new Subscription
            {
                ClientState = Settings.VerificationToken,
                ChangeType = ChangeTypes.Created,
                NotificationUrl = "https://garethj.ngrok.io/api/notifications",
                ExpirationDateTime = DateTime.UtcNow + new TimeSpan(3,0,0,0),
                Resource = "me/events" 
            };

            var subscriptionResponse = await GraphHelper.CreateSubscriptionAsync(accessToken, subscription);

            if (subscriptionResponse.Subscription != null)
            {
                subscription = subscriptionResponse.Subscription;
                userInfo.SubscriptionId = subscription.Id;
                userInfo.SubscriptionRenewalTime = subscription.ExpirationDateTime.GetValueOrDefault();

                // Write a record for the user with the subscription details
                bool success = await UserDataTable.InsertOrMergeUserInfo(userInfo);
                if (success)
                {
                    userInfo.LastError = null;
                }
                else
                {
                    userInfo.LastError = "Failed to write user's database record.";
                }
            }
            else
            {
                userInfo.LastError = subscriptionResponse.Error;
            }



            return RedirectToAction(nameof(Result), "Subscription");
        }

        void RestoreUserFromSession(ref UserInfoEntity userInfo)
        {
            var currentUser = (UserInfoEntity)Session[SessionKeys.Login.UserInfo];

            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
            {
                userInfo = currentUser;
            }
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

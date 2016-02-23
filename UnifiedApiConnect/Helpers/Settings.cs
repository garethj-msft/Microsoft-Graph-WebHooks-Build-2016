// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 

using System.Configuration;

namespace UnifiedApiConnect.Helpers
{
    public static class Settings
    {
        public static string ClientId => ConfigurationManager.AppSettings["ClientID"];
        public static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];
        public static string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"];

        public static string AzureADAuthority = @"https://login.microsoftonline.com/common";

        public static string LogoutAuthority =
            @"https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=";

        public static string MicrosoftGraphResource = @"https://graph.microsoft.com/";

        public static string CreateSubscriptionUrl = @"https://graph.microsoft.com/beta/subscriptions";

        public const string VerificationToken = "Build2016AppUniqueIdentifier";

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



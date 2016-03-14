// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. 

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace GraphWebhooksTranslator.Models
{

    public enum Language
    {
        NL,
        ES,
        FR
    }

    public class UserInfoEntity : TableEntity
    {

        private string subscriptionId;
        private Language? language = null;

        public string UserId { get; set; }

        public string RefreshToken { get; set; }

        /// <summary>
        /// Azure table does not support enumerations, so store as a string but present as an Enum.
        /// </summary>
        public Language Language
        {
            get
            {
                if (this.language.HasValue)
                {
                    return this.language.Value;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.LanguageString))
                    {
                        Language converted;
                        bool succeeded = Language.TryParse(this.LanguageString, true, out converted);
                        if (succeeded)
                        {
                            this.language = converted;
                            return this.language.Value;
                        }
                    }
                    return default(Language);
                }
            }
            set
            {
                this.language = value;
                this.LanguageString = value.ToString().ToLowerInvariant();
            }
        }

        public string LanguageString { get; set; } 

        public string SubscriptionId
        {
            get { return this.subscriptionId; }
            set
            {
                this.subscriptionId = value;
                if (value != null)
                {
                    this.PartitionKey = this.subscriptionId[0].ToString();
                    this.RowKey = this.subscriptionId.Replace('-', 'a');
                }
            }
        }

        public DateTimeOffset SubscriptionRenewalTime { get; set; }

        public UserInfoEntity()
        {
        }

        public string LastError { get; set; }
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

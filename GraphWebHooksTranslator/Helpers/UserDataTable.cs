using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using GraphWebhooksTranslator.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace GraphWebhooksTranslator.Helpers
{
	public static class UserDataTable 
	{
		private static CloudTable _userTable = null;

		private static async Task<CloudTable> GetUserTable()
		{
			if (null == _userTable)
			{
				_userTable = await TableAccess.CreateTableAsync();
			}
			return _userTable;
		}

		public static async Task<UserInfoEntity> RetrieveUserInfo(string id)
		{
			TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(id[0].ToString(), id.Replace('-','a'));
			CloudTable users = await GetUserTable();
			TableResult result = await users.ExecuteAsync(retrieveOperation);
			return result.Result as UserInfoEntity;
		}

		public static async Task<bool> InsertOrMergeUserInfo(UserInfoEntity userInfoEntity)
		{
			TableOperation insertOperation = TableOperation.InsertOrMerge(userInfoEntity);
			CloudTable users = await GetUserTable();
		    TableResult result = null;
            try
            {
		        result = await users.ExecuteAsync(insertOperation);
		    }
		    catch (StorageException)
		    {

		    }
		    return result?.Result != null && !string.IsNullOrWhiteSpace(result.Etag);
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

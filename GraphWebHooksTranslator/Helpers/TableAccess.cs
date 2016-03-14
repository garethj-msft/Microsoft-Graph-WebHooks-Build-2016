using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace GraphWebhooksTranslator.Helpers
{
    public static class TableAccess
	{
		private const string TableName = "WebHooksUser";

	    /// <summary>
		/// Create a table for the sample application to process messages in. 
		/// </summary>
		/// <returns>A CloudTable object</returns>
		internal static async Task<CloudTable> CreateTableAsync()
		{
			// Retrieve storage account information from connection string.
	        var connection = Settings.StorageConnectionString;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);
            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

			CloudTable table = tableClient.GetTableReference(TableName);
			try
			{
				await table.CreateIfNotExistsAsync();
			}
			catch (StorageException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}


			return table;
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

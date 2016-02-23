using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnifiedApiConnect.Helpers
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
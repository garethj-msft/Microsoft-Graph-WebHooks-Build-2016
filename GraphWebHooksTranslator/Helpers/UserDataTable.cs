using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UnifiedApiConnect.Models;

namespace UnifiedApiConnect.Helpers
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
		    catch (StorageException stgException)
		    {

		    }
		    return result?.Result != null && !string.IsNullOrWhiteSpace(result.Etag);
		}
	}
}
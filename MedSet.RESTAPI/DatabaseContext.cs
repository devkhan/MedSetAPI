using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MedSet.RESTAPI
{
	public class DatabaseContext
	{
		private static DatabaseContext instance;
		private static IMongoDatabase database;
		
		private DatabaseContext()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			database = client.GetDatabase("medset");
		}

		public DatabaseContext Instance
		{
			get
			{
				if (instance==null)
					instance = new DatabaseContext();
				return instance;
			}
		}

		public Task<UserModel> GetUser(string UserId)
		{
			var collection = database.GetCollection<UserModel>("users");

			var result = collection.Find(p => p.UserId == UserId);
			return result.FirstAsync();
		}

		public Task<UserModel> GetUser(string AuthProvider, string AuthID)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find(p => (p.AuthProvider == AuthProvider && p.AuthId == AuthID));
			return result.FirstAsync();
		}

		public async Task<bool> UserExists(string UserId)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find(p => p.UserId == UserId);

			long count = 0;
			try
			{
				count = await result.CountAsync();
			}
			catch (Exception e)
			{
				Debug.WriteLine("Oops! An error occured in UserExists. Error: " + e.Message);
				return false;
			}
			if (count == 0)
			{
				return false;
			}
			else
				return true;
		}

		public async void AddUser(UserModel user)
		{
			var collection = database.GetCollection<UserModel>("users");
			await collection.InsertOneAsync(user);
		}
	}
}
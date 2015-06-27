#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;

// Third-party includes.
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


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

		public static DatabaseContext Instance
		{
			get
			{
				if (instance==null)
					instance = new DatabaseContext();
				return instance;
			}
		}

		public UserModel GetUser(string UserId)
		{
			var collection = database.GetCollection<UserModel>("users");

			var result = collection.Find(p => p.UserId == UserId);

			result.FirstAsync().ConfigureAwait(false);
			return result.FirstAsync().Result;
		}

		public Task<UserModel> GetUser(string AuthProvider, string AuthID)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find(p => (p.AuthProvider == AuthProvider && p.AuthId == AuthID));
			return result.FirstAsync();
		}

		public bool UserExists(string UserId)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find<UserModel>(p => p.UserId == UserId);
			
			long count = 0;
			try
			{
				result.CountAsync().ConfigureAwait(false);
				count = result.CountAsync().Result;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Oops! An error occured in UserExists(). Error: " + e.Message);
				return false;
			}
			if (count == 0)
			{
				Debug.WriteLine("UserExists(): count == 0.");
				return false;
			}
			else
				Debug.WriteLine("UserExists(): count != 0.");
				return true;
		}

		public async Task<bool> UserExists(string AuthProvider, string AuthID)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find<UserModel>(p => (p.AuthProvider == AuthProvider && p.AuthId == AuthID));
			
			long count = 0;
			try
			{
				result.CountAsync().ConfigureAwait(false);
				count = result.CountAsync().Result;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Oops! An error occured in UserExists. Error: " + e.Message);
				return false;
			}
			if (count == 0)
			{
				Debug.WriteLine("UserExists(): count == 0.");
				return false;
			}
			else
				Debug.WriteLine("UserExists(): count != 0.");
				return true;
		}

		public async void AddUser(UserModel user)
		{
			var collection = database.GetCollection<UserModel>("users");
			await collection.InsertOneAsync(user);
		}

		public void UpdateAuthToken(UserModel user)
		{
			var collection = database.GetCollection<UserModel>("users");
			var filter = Builders<UserModel>.Filter.Eq("UserId", user.UserId);
			var update = Builders<UserModel>.Update.Set("AuthToken", user.AuthToken);
			collection.UpdateOneAsync(filter, update).ConfigureAwait(false);
			collection.UpdateOneAsync(filter, update);
		}

		public List<ReportModel> GetReports(string UserId)
		{
			var collection = database.GetCollection<ReportModel>("reports");
			collection.Find(Builders<ReportModel>.Filter.Where(e => e.UserId == UserId)).ToListAsync().ConfigureAwait(false);
			var result = collection.Find(Builders<ReportModel>.Filter.Where(e => e.UserId == UserId)).ToListAsync().Result;
			return result;
		}

		public bool AddReport(ReportModel report)
		{
			if (UserExists(report.UserId))
			{
				// Create new report ID using a combined hash of user-id and
				// timiestamp.
				report.ReportID = String.Format("{0:X}", (report.UserId+report.Timestamp.ToString()).GetHashCode());
				var collection = database.GetCollection<ReportModel>("reports");
				collection.InsertOneAsync(report).ConfigureAwait(false);
				collection.InsertOneAsync(report);
				return true;
			}
			else
			{
				// User doesn't exixts, hope to provide some meaningful error
				// messages in the future.
				return false;
			}
		}

		public bool ModifyReport(ReportModel report)
		{
			if (ReportExists(report.ReportID))
			{
				var collection = database.GetCollection<ReportModel>("reports");
				collection.DeleteOneAsync(Builders<ReportModel>.Filter.Eq("ReportID", report.ReportID)).ConfigureAwait(false);
				collection.DeleteOneAsync(Builders<ReportModel>.Filter.Eq("ReportID", report.ReportID));
				collection.InsertOneAsync(report).ConfigureAwait(false);
				collection.InsertOneAsync(report);
				return true;
			}
			else
			{
				// Again, a better error in future.
				return false;
			}
		}

		public bool ReportExists(string ReportID)
		{
			var collection = database.GetCollection<ReportModel>("reports");
			var result = collection.Find<ReportModel>(p => p.ReportID == ReportID);

			long count = 0;
			try
			{
				result.CountAsync().ConfigureAwait(false);
				count = result.CountAsync().Result;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Oops! An error occured in ReportsExists(). Error: " + e.Message);
				return false;
			}
			if (count == 0)
			{
				Debug.WriteLine("UserExists(): count == 0.");
				return false;
			}
			else
				Debug.WriteLine("UserExists(): count != 0.");
			return true;
		}

		public bool TokenValid(string UserId, string access_token)
		{
			if (UserExists(UserId))
			{
				var user = GetUser(UserId);
				if (user.AuthToken.Key == access_token && !Utils.Instance.TokenExpired(user.AuthToken.Value))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}
	}
}
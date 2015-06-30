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
		/// <summary>
		/// The instance
		/// </summary>
		private static DatabaseContext instance;
		/// <summary>
		/// The database
		/// </summary>
		private static IMongoDatabase database;

		/// <summary>
		/// Prevents a default instance of the <see cref="DatabaseContext"/> class from being created.
		/// </summary>
		private DatabaseContext()
		{
			var client = new MongoClient("mongodb://localhost:27017");
			database = client.GetDatabase("medset");
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static DatabaseContext Instance
		{
			get
			{
				if (instance==null)
					instance = new DatabaseContext();
				return instance;
			}
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <param name="UserId">The user identifier.</param>
		/// <returns></returns>
		public UserModel GetUser(string UserId)
		{
			var collection = database.GetCollection<UserModel>("users");

			var result = collection.Find(p => p.UserId == UserId);

			result.FirstAsync().ConfigureAwait(false);
			return result.FirstAsync().Result;
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <param name="AuthProvider">The authentication provider.</param>
		/// <param name="AuthID">The authentication identifier.</param>
		/// <returns></returns>
		public Task<UserModel> GetUser(string AuthProvider, string AuthID)
		{
			var collection = database.GetCollection<UserModel>("users");
			var result = collection.Find(p => (p.AuthProvider == AuthProvider && p.AuthId == AuthID));
			return result.FirstAsync();
		}

		/// <summary>
		/// Users the exists.
		/// </summary>
		/// <param name="UserId">The user identifier.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Users the exists.
		/// </summary>
		/// <param name="AuthProvider">The authentication provider.</param>
		/// <param name="AuthID">The authentication identifier.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Adds the user.
		/// </summary>
		/// <param name="user">The user.</param>
		public async void AddUser(UserModel user)
		{
			var collection = database.GetCollection<UserModel>("users");
			await collection.InsertOneAsync(user);
		}

		/// <summary>
		/// Updates the authentication token.
		/// </summary>
		/// <param name="user">The user.</param>
		public void UpdateAuthToken(UserModel user)
		{
			var collection = database.GetCollection<UserModel>("users");
			var filter = Builders<UserModel>.Filter.Eq("UserId", user.UserId);
			var update = Builders<UserModel>.Update.Set("AuthToken", user.AuthToken);
			collection.UpdateOneAsync(filter, update).ConfigureAwait(false);
			collection.UpdateOneAsync(filter, update);
		}

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <param name="UserId">The user identifier.</param>
		/// <returns></returns>
		public List<ReportModel> GetReports(string UserId)
		{
			var collection = database.GetCollection<ReportModel>("reports");
			collection.Find(Builders<ReportModel>.Filter.Where(e => e.UserId == UserId)).ToListAsync().ConfigureAwait(false);
			var result = collection.Find(Builders<ReportModel>.Filter.Where(e => e.UserId == UserId)).ToListAsync().Result;
			return result;
		}

		/// <summary>
		/// Adds the report.
		/// </summary>
		/// <param name="report">The report.</param>
		/// <returns></returns>
		public bool AddReport(ref ReportModel report)
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

		/// <summary>
		/// Modifies the report.
		/// </summary>
		/// <param name="report">The report.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Reports the exists.
		/// </summary>
		/// <param name="ReportID">The report identifier.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Tokens the valid.
		/// </summary>
		/// <param name="UserId">The user identifier.</param>
		/// <param name="access_token">The access_token.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the report.
		/// </summary>
		/// <param name="ReportID">The report identifier.</param>
		/// <returns></returns>
		public ReportModel GetReport(string ReportID)
		{
			var collection = database.GetCollection<ReportModel>("reports");

			var result = collection.Find(p => p.ReportID == ReportID);

			result.FirstAsync().ConfigureAwait(false);
			return result.FirstAsync().Result;
		}
	}
}
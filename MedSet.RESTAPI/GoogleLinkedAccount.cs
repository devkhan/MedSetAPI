using Nancy;
using Nancy.SimpleAuthentication;
using Nancy.Serialization.JsonNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Diagnostics;
using MongoDB.Bson;
using System.Net;

namespace MedSet.RESTAPI
{
	public class GoogleLinkedAccount
	{
		private NancyModule nancyModule;
		private AuthenticateCallbackData model;
		private LoginRequestModel loginRequestModel;
		public static string TOKENINFO_ENDPOINT = "https://www.googleapis.com/oauth2/v3/tokeninfo";

		public GoogleLinkedAccount(NancyModule nancyModule, AuthenticateCallbackData model)
		{
			this.model = model;
			this.nancyModule = nancyModule;
		}

		public GoogleLinkedAccount(LoginRequestModel loginRequestModel)
		{
			this.loginRequestModel = loginRequestModel;
		}

		public dynamic Respond()
		{
			var task = DatabaseContext.Instance.UserExists(model.AuthenticatedClient.ProviderName,
			model.AuthenticatedClient.UserInformation.Email);
			// task.RunSynchronously(); // Causing Exception.
			task.ConfigureAwait(false);
			var result = task.Result;
			Debug.WriteLine("UserExists: " + result);
			if (result)
			{
				Debug.WriteLine("User exists, now retrieving user.");
				var userTask = DatabaseContext.Instance.GetUser(model.AuthenticatedClient.ProviderName, model.AuthenticatedClient.UserInformation.Email);
				UserModel user = userTask.Result;
				if (!Utils.Instance.TokenExpired(user.AuthToken.Value)) //IF the AuthToken is not expired.
				{
					// Retrieving existing AuthToken.
					var respone = new RegisterLoginModel(user.UserId, user.AuthToken.Key, Utils.Instance.SecondsfromNow(user.AuthToken.Value));
					return JsonConvert.SerializeObject(respone);
				}
				else
				{
					// Updating AuthToken+Timestamp.
					string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
					user.AuthToken = new KeyValuePair<string, DateTime>(token, DateTime.Now);
					DatabaseContext.Instance.ModifyUser(user);
					var response = new RegisterLoginModel(user.UserId, user.AuthToken.Key, Utils.Instance.SecondsfromNow(user.AuthToken.Value));
					return JsonConvert.SerializeObject(response);
				}
			}
			else
			{
				// Creating new user.
				Debug.WriteLine("User doesn't exists, new user being created.");
				UserModel newUser = new UserModel()
				{
					UserId = Guid.NewGuid().ToString(),
					AuthProvider = model.AuthenticatedClient.ProviderName,
					AuthId = model.AuthenticatedClient.UserInformation.Email,
					AuthToken = new KeyValuePair<string,DateTime>(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), DateTime.Now.AddDays(1))
				};
				Debug.WriteLine(newUser);
				DatabaseContext.Instance.AddUser(newUser);
				var response = new RegisterLoginModel()
				{
					user_id = newUser.UserId,
					auth_token = newUser.AuthToken.Key,
					seconds = Utils.Instance.SecondsfromNow(newUser.AuthToken.Value)
				};
				return JsonConvert.SerializeObject(response);
			}
		}

		public string ValidateToken()
		{
			string response = String.Empty;
			WebRequest req = System.Net.WebRequest.Create(TOKENINFO_ENDPOINT + "?id_token=" + loginRequestModel.id_token);
			try
			{
				WebResponse resp = req.GetResponse();
				using (System.IO.Stream stream = resp.GetResponseStream())
				{
					using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
					{
						response = streamReader.ReadToEnd();
						streamReader.Close();
					}
				}
			}
			catch (ArgumentException ex)
			{
				throw new CustomException("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' ::", ex);
			}
			catch (WebException ex)
			{
				throw new CustomException("HTTP_ERROR :: WebException raised! ::", ex);
			}
			catch (Exception ex)
			{
				throw new CustomException("HTTP_ERROR :: Exception raised! ::", ex);
			}

			return response;
		}
	}
}
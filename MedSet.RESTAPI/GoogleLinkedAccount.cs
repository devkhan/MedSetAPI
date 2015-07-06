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
using System.IO;
using System.Collections.Specialized;

namespace MedSet.RESTAPI
{
	public class GoogleLinkedAccount
	{
		private NancyModule nancyModule;
		private AuthenticateCallbackData model;
		private LoginRequestModel loginRequestModel;
		public static string TOKENINFO_ENDPOINT = @"https://www.googleapis.com/oauth2/v3/tokeninfo";
		public static string REFRESHTOKEN_ENDPOINT = @"https://www.googleapis.com/oauth2/v3/token";
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
			task.ConfigureAwait(false); // For running async methods synchronously.
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
					DatabaseContext.Instance.UpdateAuthToken(user);
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
			// Below code check the ID token against the Google servers for
			// verification of user.
			String response = String.Empty;
			Uri uri = new Uri(TOKENINFO_ENDPOINT + "?id_token=" + loginRequestModel.id_token);
			WebRequest req = WebRequest.Create(uri);
			WebResponse resp;
			try
			{
				resp = req.GetResponse();
				using (Stream stream = resp.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(stream))
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
				Debug.WriteLine("Request URI: " + req.RequestUri);
				throw new CustomException("HTTP_ERROR :: WebException raised! ::", ex);
			}
			catch (Exception ex)
			{
				throw new CustomException("HTTP_ERROR :: Exception raised! ::", ex);
			}

			
			return response;
		}

		public void SaveRefreshToken()
		{
			// Code for exchanging auth code for refresh token. The result of
			// the following request is recieved at the redirect uri's.
			Uri uri = new Uri(REFRESHTOKEN_ENDPOINT);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
			WebResponse resp;
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
			outgoingQueryString.Add("code", loginRequestModel.server_auth_code);
			outgoingQueryString.Add("client_id", "870300371040-242h9v895pnjmq7ps97tacss0nn4ple3.apps.googleusercontent.com");
			outgoingQueryString.Add("client_secret", "jo8kn0v0kavKEsMmj3zDNZGH");
			outgoingQueryString.Add("redirect_uri", @"urn:ietf:wg:oauth:2.0:oob");
			outgoingQueryString.Add("grant_type", "authorization_code");
			string postdata = outgoingQueryString.ToString();
			req.GetRequestStream().Write(System.Text.Encoding.ASCII.GetBytes(postdata), 0, System.Text.Encoding.ASCII.GetBytes(postdata).Length);
			
			string result = String.Empty;
			try
			{
				resp = req.GetResponse();
				using (Stream stream = resp.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						result = streamReader.ReadToEnd();
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
			finally
			{
				loginRequestModel.server_auth_code = (string)BsonDocument.Parse(result)["refresh_token"];
			}

		}
		public dynamic RespondToIDToken()
		{
			BsonDocument tokeninfo;
			try
			{
				SaveRefreshToken();
				tokeninfo = BsonDocument.Parse(ValidateToken());
				loginRequestModel.auth_id = (string)tokeninfo["email"];
				Debug.WriteLine("Response from Google tokeninfo endpoint: " + tokeninfo);
			}
			catch (Exception ex)
			{
				return "Oops, an error occured. Details: " + ex.Message;
			}
			if (tokeninfo.Contains("error_description"))
			{
				// ID token invalid.
				return new BsonDocument{
					{"status", "error"},
					{"description", "Invalid ID token"}
				};
			}
			else if ((string)tokeninfo["email"]!=loginRequestModel.auth_id)
			{
				// Email provided by the client and auth provider doesn't match.
				return new BsonDocument{
					{"status", "error"},
					{"description", "Email ID mismatch"}
				};
			}
			else
			{
				var task = DatabaseContext.Instance.UserExists(loginRequestModel.auth_provider,loginRequestModel.auth_id);
				task.ConfigureAwait(false); // For running async methods synchronously.
				var result = task.Result;
				Debug.WriteLine("UserExists: " + result);
				if (result)
				{
					Debug.WriteLine("User exists, now retrieving user.");
					var userTask = DatabaseContext.Instance.GetUser(loginRequestModel.auth_provider, loginRequestModel.auth_id);
					UserModel user = userTask.Result;
					// Updating AuthToken+Timestamp.
					string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
					user.AuthToken = new KeyValuePair<string, DateTime>(token, DateTime.Now);
					DatabaseContext.Instance.UpdateAuthToken(user);
					var response = new RegisterLoginModel(user.UserId, user.AuthToken.Key, Utils.Instance.SecondsfromNow(user.AuthToken.Value));
					return JsonConvert.SerializeObject(response);
				}
				else
				{
					// Creating new user.
					Debug.WriteLine("User doesn't exists, new user being created.");
					UserModel newUser = new UserModel()
					{
						UserId = Guid.NewGuid().ToString(),
						AuthProvider = loginRequestModel.auth_provider,
						AuthId = loginRequestModel.auth_id,
						IdToken = loginRequestModel.id_token,
						AuthToken = new KeyValuePair<string, DateTime>(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), DateTime.Now.AddDays(1)),
						AuthCode = loginRequestModel.server_auth_code
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
		}
	}
}
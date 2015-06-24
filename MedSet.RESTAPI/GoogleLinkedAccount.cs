﻿using Nancy;
using Nancy.SimpleAuthentication;
using Nancy.Serialization.JsonNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MedSet.RESTAPI
{
	public class GoogleLinkedAccount
	{
		private NancyModule nancyModule;
		private AuthenticateCallbackData model;		

		public GoogleLinkedAccount(NancyModule nancyModule, AuthenticateCallbackData model)
		{
			this.model = model;
			this.nancyModule = nancyModule;
		}

		public dynamic Respond()
		{
			var task = DatabaseContext.Instance.UserExists(model.AuthenticatedClient.ProviderName,
			model.AuthenticatedClient.UserInformation.Id);
			// task.RunSynchronously(); // Causing Exception.
			task.ConfigureAwait(false);
			var result = task.Result;
			Debug.WriteLine("UserExists: " + result);
			if (result)
			{
				Debug.WriteLine("User exists, now retrieving user.");
				var userTask = DatabaseContext.Instance.GetUser(model.AuthenticatedClient.ProviderName, model.AuthenticatedClient.UserInformation.Id);
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
					AuthId = model.AuthenticatedClient.UserInformation.Id,
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
	}
}
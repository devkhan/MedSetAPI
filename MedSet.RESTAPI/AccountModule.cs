using MongoDB.Bson;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class AccountModule : NancyModule
	{
		public AccountModule() : base ("/api/account")
		{
			Get["/"] = _ =>
				{
					return "Hello, this is the root for the API auth.";
				};

			Post["/tokensignin"] = _ =>
				{
					// TO-DO: Getting ID token(not OAuth access token) from app,
					// validate the token against provider and check if user
					// exists and then, register or login the user.
					var requestBody = this.Request.Form;
					requestBody = this.Request.Body;
					var signinRequestBody = new LoginRequestModel()
					{
						auth_id = this.Request.Form["auth_id"],
						auth_provider = this.Request.Form["auth_provider"],
						id_token = this.Request.Form["id_token"],
						server_auth_code = this.Request.Form["server_auth_code"]
					};
					var googleLinkedModel = new GoogleLinkedAccount(signinRequestBody);
					try
					{
						return googleLinkedModel.RespondToIDToken();
					}
					catch (Exception ex)
					{
						return ("Oops, an error occured. Details: " + ex.Message);
					}

				};

			Get["/refreshtokencallback"] = _ =>
				{
					return this.Request.Body.AsString();
				};

			Post["/refreshtokencallback"] = _ =>
				{
					return this.Request.Body.AsString();
				};
		}
	}
}
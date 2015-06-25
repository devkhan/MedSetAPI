using Nancy;
using Nancy.SimpleAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class MedSetAuthenticationCallbackProvider : IAuthenticationCallbackProvider
	{
		public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
		{
			switch((string)nancyModule.Context.Request.Query.providerkey)
			{
				case "google":
					var googleLinkedAccount = new GoogleLinkedAccount(nancyModule, model);
					return model.AuthenticatedClient.ToString() + googleLinkedAccount.Respond().ToString();
				case "facebook":
					return "Sorry, facebook is not yet supported. Check again later.";
				default:
					return "No valid authentication provider found!!!";
			}
		}

		public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
		{
			return "Yay, an error occured. Error: " + errorMessage;
		}
	}
}
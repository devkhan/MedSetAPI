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
			return "Yay, you're authenticated" + "Client: " + model.AuthenticatedClient + "Return URI: " + model.ReturnUrl + "Request: " ;
		}

		public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
		{
			return "Yay, an error occured. Error: " + errorMessage;
		}
	}
}
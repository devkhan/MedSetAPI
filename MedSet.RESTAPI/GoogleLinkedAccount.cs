using Nancy;
using Nancy.SimpleAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
	}
}
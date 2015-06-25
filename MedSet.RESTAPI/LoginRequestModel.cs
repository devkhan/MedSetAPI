using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class LoginRequestModel
	{
		// This will be the ID token provided by the client app retrieved from
		// Google APIs and contains important info, ex. email, client id, etc..
		public string id_token { get; set; }
		// Auth provider for searching in our database.
		public string auth_provider { get; set; }
		// Auth ID for searching in our database. Email Id in our case.
		public string auth_id { get; set; }
	}
}
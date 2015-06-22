using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class AccountModule : NancyModule
	{
		public AccountModule() : base ("/api/Account")
		{
			Get["/"] = _ =>
				{
					return "Hello, this is the root for the API auth.";
				};
		}
	}
}
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class APIModule : NancyModule
	{
		public APIModule()
		{
			Get["/nothing"] = parameters =>
				{
					return "Nothing";
				};
		}

		private dynamic APISubmodule(dynamic parameters)
		{
			throw new NotImplementedException();
		}
	}
}
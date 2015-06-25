using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class CustomException : Exception
	{
		public CustomException()
			: base()
		{
		}

		public CustomException(string message)
			: base(message)
		{
		}

		public CustomException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		//...other constructors with parametrized messages for localization if needed
	}
}
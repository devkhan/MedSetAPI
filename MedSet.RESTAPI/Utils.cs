using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class Utils
	{
		private Utils()
		{
			// Required empty private constructor for singleton.
		}

		private static Utils instance;
		public static Utils Instance
		{
			get
			{
				if (instance==null)
				{
					instance = new Utils();
				}
				return instance;
			}
		}
		public long SecondsfromNow(DateTime dateTime)
		{
			return (long)(dateTime - DateTime.Now).TotalSeconds;
		}

		public bool TokenExpired(DateTime dateTime)
		{
			return (SecondsfromNow(dateTime) <= 0) ? true : false;
		}
	}
}
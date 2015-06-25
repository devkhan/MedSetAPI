using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class Utils
	{
		private static FileStream F = new FileStream(@"E:\Git\MedSetAPI\MedSet.RESTAPI\debug_log.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
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

		public enum Gender
		{
			Female,
			Male,
			Transgender,
			Other,
			Unspecified
		}
	}
}
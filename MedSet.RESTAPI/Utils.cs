using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MedSet.RESTAPI
{
	public class Utils
	{
		private static FileStream F = new FileStream(@"E:\Git\MedSetAPI\MedSet.RESTAPI\debug_log.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
		/// <summary>
		/// The fil e_ bas e_ path
		/// </summary>
		public static string FILE_BASE_PATH = @"C:\files\medset\users\";
		private Utils()
		{
			// Required empty private constructor for singleton.
		}

		private static Utils instance;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
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
		/// <summary>
		/// Secondsfroms the now.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <returns></returns>
		public long SecondsfromNow(DateTime dateTime)
		{
			return (long)(dateTime - DateTime.Now).TotalSeconds;
		}

		/// <summary>
		/// Tokens the expired.
		/// </summary>
		/// <param name="dateTime">The date time.</param>
		/// <returns></returns>
		public bool TokenExpired(DateTime dateTime)
		{
			return (SecondsfromNow(dateTime) <= 0) ? true : false;
		}

		/// <summary>
		/// 
		/// </summary>
		public enum Gender
		{
			Female,
			Male,
			Transgender,
			Other,
			Unspecified
		}

		/// <summary>
		/// Saves the image from string.
		/// </summary>
		/// <param name="encodedString">The encoded string.</param>
		/// <param name="ReportID">The report identifier.</param>
		public void SaveImageFromString(string encodedString, string ReportID)
		{
			var bytes = Convert.FromBase64String(encodedString);
			using (var imageFile = new FileStream(FILE_BASE_PATH+ReportID+".jpg", FileMode.Create))
			{
				imageFile.Write(bytes, 0, bytes.Length);
				imageFile.Flush();
			}
		}
	}
}
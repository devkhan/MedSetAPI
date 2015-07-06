using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace MedSet.RESTAPI
{
	/// <summary>
	/// 
	/// </summary>
	public class Utils
	{
		private static FileStream F = new FileStream(@"E:\Git\MedSetAPI\MedSet.RESTAPI\debug_log.txt", FileMode.OpenOrCreate, FileAccess.Write);
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
			return (SecondsfromNow(dateTime) <= -(24*60*60)) ? true : false;
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

		/// <summary>
		/// Calculates the m d5 hash.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Converts DateTime value to UNIX timestamp.
		/// </summary>
		/// <param name="value">The DateTime variable.</param>
		/// <returns></returns>
		public static long ConvertToTimestamp(DateTime value)
		{
			//create Timespan by subtracting the value provided from
			//the Unix Epoch
			TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

			//return the total seconds (which is a UNIX timestamp)
			return (long)span.TotalSeconds;
		} 
	}
}
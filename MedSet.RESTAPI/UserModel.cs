using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class UserModel
	{
		public ObjectId _id { get; set; }

		// User Id alloted by our app.
		[Required]
		public string UserId { get; set; }

		// Authentication provider. Example: Google, Facebook.
		[Required]
		public string AuthProvider { get; set; }

		// User Id given by the authentication provider. Example - 
		[Required]
		public string AuthId { get; set; }

		// Code returned by OAuth provider.
		public string AuthCode { get; set; }

		// ID token retrieved from the user.
		public string IdToken { get; set; }

		// Authorization code provided by the our app along with timestamp.
		public KeyValuePair<string, DateTime> AuthToken { get; set; }


		/// <summary>
		/// Contains personal information collected from external auth provider API and as given by user.
		/// 
		/// Keys			-	Type
		/// =================================
		/// FirstName			string
		/// MiddleName			string
		/// LastName			string
		/// Gender				enum Utils.Gender
		/// DOB					DateTime
		/// Address				string
		/// City				string
		/// State				string
		/// Country				string
		/// Pincode				int
		/// Mobile				string
		/// EMailID				string/EmailID
		/// 
		/// </summary>
		public Dictionary<string, BsonValue> PersonalDetails { get; set; }

		/// <summary>
		/// Contains information related to health as given by the user.
		/// 
		/// Keys			-	Type
		/// =================================
		/// Diabetic			bool
		/// HeartPatient		bool
		/// 
		/// </summary>
		public Dictionary<string, BsonValue> MedicalDetails { get; set; }
		

	}
}
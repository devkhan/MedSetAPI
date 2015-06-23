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
		// Authorization code provided by the our app along with timestamp.
		public KeyValuePair<string, DateTime> AuthToken { get; set; }
		public Dictionary<string, BsonValue> PersonalDetails { get; set; }
		public Dictionary<string, BsonValue> MedicalDetails { get; set; }

	}
}
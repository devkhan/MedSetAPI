using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedSet.RESTAPI
{
	public class UserModel
	{
		public ObjectId _id { get; set; }

		// User Id alloted by our app.
		public string UserId { get; set; }
		// Authentication provider. Example: Google, Facebook.
		public string AuthProvider { get; set; }
		// User Id given by the authentication provider. Example - 
		public string AuthId { get; set; }
		// Authorization code provided by the OAUth provider.
		public string AuthCode { get; set; }
		public Dictionary<string, BsonValue> PersonalDetails { get; set; }
		public Dictionary<string, BsonValue> MedicalDetails { get; set; }

	}
}
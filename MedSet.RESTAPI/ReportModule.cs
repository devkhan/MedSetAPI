using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Nancy;
using Newtonsoft.Json;
using Nancy.Extensions;

namespace MedSet.RESTAPI
{
	public class ReportModule : NancyModule
	{
		public ReportModule() : base("/api/user")
		{
			Post["/{user_id:guid}/reports/count/{count:int}"] = parameters =>
				{
					dynamic request = JsonConvert.DeserializeObject(this.Request.Body.AsString());
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)request.access_token))
					{
						var reports = DatabaseContext.Instance.GetReports(parameters.user_id.ToString());
						return new BsonDocument{
							{"reports", JsonConvert.SerializeObject(reports)}
						};
					}
					else
					{
						return new BsonDocument{
							{"status","error"},
							{"description","either the user_id doesn't exists or your access_token is invalid."}
						};
					}
				};

			Post["/{user_id:guid}/reports/add"] = parameters =>
				{

					return "Not fully implemented yet!!!";
				};
		}
	}
}
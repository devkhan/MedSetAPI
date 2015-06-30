using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Nancy;
using Newtonsoft.Json;
using Nancy.Extensions;
using System.IO;

namespace MedSet.RESTAPI
{
	public class ReportModule : NancyModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportModule"/> class.
		/// </summary>
		public ReportModule() : base("/api/user")
		{
			Post["/{user_id:guid}/reports/count/{count:int}"] = parameters =>
				{
					dynamic request = JsonConvert.DeserializeObject(this.Request.Body.AsString());
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)request.access_token))
					{
						var reports = DatabaseContext.Instance.GetReports(parameters.user_id.ToString());
						return new BsonDocument
						{
							{"status", "fine"},
							{"reports", JsonConvert.SerializeObject(reports)}
						};
					}
					else
					{
						return new BsonDocument
						{
							{"status","error"},
							{"description","either the user_id doesn't exists or your access_token is invalid."}
						};
					}
				};

			Post["/{user_id:guid}/reports/add"] = parameters =>
				{
					dynamic request = JsonConvert.DeserializeObject(this.Request.Body.AsString());
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)request.access_token))
					{
						var reportModel = new ReportModel
						{
							UserId = (String)parameters.user_id,
							Title = request.report.title,
							Timestamp = DateTime.Parse(request.report.Timestamp),
							Type = request.report.Type,
							FilesNumber = request.report.FilesNumber
						};
						if(DatabaseContext.Instance.AddReport(ref reportModel))
						{
							return new BsonDocument
							{
								{"report_id", reportModel.ReportID},
								{"status", "fine"},
								{"description", "report metadata added successfully, now you can upload files."}
							};
						}
					}
					return new BsonDocument
					{
						{"status","error"},
						{"description","either the user_id doesn't exists or your access_token is invalid."}
					};
				};

			Put["/{user_id:guid}/reports/{report_id:string}/{file:int}"] = parameters =>
				{
					dynamic request = JsonConvert.DeserializeObject(this.Request.Body.AsString());
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)request.access_token))
					{
						if (DatabaseContext.Instance.ReportExists(parameters.report_id))
						{
							if (DatabaseContext.Instance.GetReport(parameters.report_id).FilesNumber<parameters.file && parameters.file>0)
							{
								// TO-DO: Recieve uploaded file and save it
								// somewhere on the disk with a properly
								// formatted path.
								var file = this.Request.Files.FirstOrDefault();
								var filePath = Path.Combine(Utils.FILE_BASE_PATH + (string)parameters.user_id + @"\reports\" + parameters.report_id + "_" + (string)parameters.file);
								using (var fileStream = new FileStream(filePath, FileMode.Create))
								{
									
								}
							}
						}
					}
					return new BsonDocument
					{
						{"status","error"},
						{"description","either the user_id doesn't exists or your access_token is invalid."}
					};
				};
		}
	}
}
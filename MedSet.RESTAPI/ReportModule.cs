﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Nancy;
using Newtonsoft.Json;
using Nancy.Extensions;
using System.IO;
using Nancy.IO;

namespace MedSet.RESTAPI
{
	public class ReportModule : NancyModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportModule"/> class.
		/// </summary>
		public ReportModule() : base("/api/user")
		{
			StaticConfiguration.DisableErrorTraces = false;
			Get["/{user_id:guid}/reports/count/{count:int}"] = parameters =>
				{
					var access_token = Request.Headers.Authorization.Split(' ')[1];
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, access_token))
					{
						var reports = DatabaseContext.Instance.GetReports(parameters.user_id.ToString());
						return Response.AsJson(new Dictionary<string, object>
							{
								{"status", "fine"},
								{"reports", JsonConvert.SerializeObject(reports)}
							},
							HttpStatusCode.OK);
					}
					else
					{
						return Response.AsJson(new Dictionary<string, object>
							{
								{"status","error"},
								{"description","either the user_id doesn't exists or your access_token is invalid."}
							},
							HttpStatusCode.Unauthorized);
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
							Timestamp = DateTime.Now,
							Type = (ReportModel.ReportType)Enum.Parse(typeof(ReportModel.ReportType), (string)request.report.Type, true),
							FilesNumber = request.report.FilesNumber
						};
						if(DatabaseContext.Instance.AddReport(ref reportModel))
						{
							return Response.AsJson(new Dictionary<string, object>
								{
									{"report_id", reportModel.ReportID},
									{"status", "fine"},
									{"description", "report metadata added successfully, now you can upload files."}
								},
								HttpStatusCode.OK);
						}
					}
					return Response.AsJson(new Dictionary<string, object>
					{
						{"status","error"},
						{"description","either the user_id doesn't exists or your access_token is invalid."}
					},
					HttpStatusCode.Unauthorized);
				};

			Put["/{user_id:guid}/reports/{report_id}/{file:int}"] = parameters =>
				{
					var access_token = Request.Headers.Authorization.Split(' ')[1];
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)access_token))
					{
						if (DatabaseContext.Instance.ReportExists(parameters.report_id))
						{
							if (DatabaseContext.Instance.GetReport(parameters.report_id).FilesNumber>parameters.file && parameters.file>0)
							{
								// Recieve uploaded file and save it somewhere
								// on the disk with a properly formatted path.
								string filePath = Path.Combine(Utils.FILE_BASE_PATH + (string)parameters.user_id + @"\reports\");

								var temp = !Directory.Exists(filePath)? Directory.CreateDirectory(filePath): null;
								filePath += parameters.report_id + "_" + 
											(string)parameters.file + "." + 
											Request.Headers.ContentType.Split('/')[1];
								
								using (var fileStream = new FileStream(filePath, FileMode.Create))
								{
									Request.Body.CopyTo(fileStream);
									return Response.AsJson(new Dictionary<string, object>
										{
											{"Status", "ok"},
											{"description", "image uploaded succesfully"}
										});
								}
							}
						}
					}
					return Response.AsJson(new Dictionary<string, object>
						{
							{"status","error"},
							{"description","either the user_id doesn't exists or your access_token is invalid."}
						},
						HttpStatusCode.Unauthorized);
				};

			Get["/{user_id:guid}/reports/{report_id}/{file:int}"] = parameters =>
				{
					var access_token = Request.Headers.Authorization.Split(' ')[1];
					if (DatabaseContext.Instance.TokenValid((string)parameters.user_id, (string)access_token))
					{
						if (DatabaseContext.Instance.ReportExists(parameters.report_id))
						{
							if (DatabaseContext.Instance.GetReport(parameters.report_id).FilesNumber > parameters.file && parameters.file > 0)
							{
								// Recieve uploaded file and save it somewhere
								// on the disk with a properly formatted path.
								string filePath = Path.Combine(Utils.FILE_BASE_PATH + (string)parameters.user_id + @"\reports\");

								
								filePath += parameters.report_id + "_" +
											(string)parameters.file + "." +
											Request.Headers.ContentType.Split('/')[1];
								return Response.AsFile(filePath);
							}
							else
							{
								return Response.AsJson(new Dictionary<string, object>
								{
									{"status", "error"},
									{"description", "file number out of range"}
								},
								HttpStatusCode.RequestedRangeNotSatisfiable);
							}
						}
						else
						{
							return Response.AsJson(new Dictionary<string, object>
								{
									{"status", "error"},
									{"description", "report_id doesn't exists"}
								},
								HttpStatusCode.NotFound);
						}
					}
					else
					{
						return Response.AsJson(new Dictionary<string, object>
						{
							{"status","error"},
							{"description","either the user_id doesn't exists or your access_token is invalid."}
						},
						HttpStatusCode.Unauthorized);
					}
				};
					
		}
	}
}
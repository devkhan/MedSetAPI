using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MedSet.RESTAPI
{
	public class ReportModel
	{
		public ObjectId _id { get; set; }

		// Unique report ID.
		[Required]
		public string ReportID { get; set; }
		public string UserId { get; set; }
		// Report title.
		public string Title { get; set; }
		// Timestamp of the report.
		[Required]
		public DateTime Timestamp { get; set; }
		// Report type.
		public ReportType Type { get; set; }
		// Main report data - pdf file, image file, text data.
		public string data { get; set; }

		// Report type.
		public enum ReportType
		{
			PDF,
			Image,
			Text
		}
	}
}
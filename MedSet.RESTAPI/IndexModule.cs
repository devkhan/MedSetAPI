namespace MedSet.RESTAPI
{
	using Nancy;

	public class IndexModule : NancyModule
	{
		public IndexModule()
		{
			Get["/"] = parameters =>
			{
				this.Context.Trace.TraceLog.WriteLog(s => s.AppendLine("Root path was called"));
				return View["index"];
			};

			Get["/hello"] = parameters =>
				{
					return "Hello Nancy!";
				};
		}
	}
}
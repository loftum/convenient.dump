using System;

namespace Convenient.Dump.Core
{
	internal class ExceptionModel
	{
		public string Type { get; set; }
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public ExceptionModel InnerException { get; set; }

		public ExceptionModel(Exception ex)
		{
			Type = ex?.GetType().Name;
			Message = ex?.Message ?? "There is no spoon";
			StackTrace = ex?.StackTrace;
			InnerException = ex.InnerException == null ? null : new ExceptionModel(ex.InnerException);
		}
	}
}
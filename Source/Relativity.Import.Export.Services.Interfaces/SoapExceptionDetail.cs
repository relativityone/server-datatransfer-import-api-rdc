﻿namespace Relativity.Import.Export.Services
{
	public class SoapExceptionDetail
	{
		public string[] Details { get; set; }

		public string ExceptionFullText { get; set; }

		public string ExceptionMessage { get; set; }

		public string ExceptionTrace { get; set; }

		public string ExceptionType { get; set; }
	}
}
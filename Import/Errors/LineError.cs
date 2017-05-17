using System.Collections;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class LineError
	{
		public string Message { get; set; }

		public int LineNumber { get; set; }

		public ErrorType ErrorType { get; set; }

		//TODO: Used only in ImportAPI
		public string Identifier => string.Empty;

		public Hashtable ToHashtable()
		{
			return new Hashtable
			{
				{"Message", Message},
				{"Line Number", LineNumber},
				{"Identifier", Identifier}
			};
		}
	}
}
using System.Collections;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class LineError
	{
		public string Message { get; set; }

		public int LineNumber { get; set; }

		public ErrorType ErrorType { get; set; }

		/// <summary>
		///     Identifier is only used in ImportAPI - for RDC it's always empty string
		/// </summary>
		public string Identifier { get; set; } = string.Empty;

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
using System.Collections;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class LineError
	{
		private string _message;

		public string Message
		{
			get { return _message; }
			set { _message = value ?? "Unknown error occurred"; }
		}

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
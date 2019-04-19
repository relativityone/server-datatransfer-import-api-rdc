namespace Relativity.Import.Export.Services
{
	using System;

	public static class Constants
	{
		public const string LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN = "#KCURA99DF2F0FEB88420388879F1282A55760#";

		/// <summary>
		/// The default field delimiter to use when no other value is set.
		/// </summary>
		/// <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string DEFAULT_FIELD_DELIMITER = "þþKþþ";

		/// <summary>
		/// The value used at the end of a line by the image loader.
		/// </summary>
		/// <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string ENDLINETERMSTRING = DEFAULT_FIELD_DELIMITER + Microsoft.VisualBasic.Constants.vbCrLf;
		public const string LICENSE_AGREEMENT_TEXT = "The programs included herein are subject to a restricted use license and can only be used in conjunction with this application.";
		public const string Name = "Name";
		public const string Notes = "Notes";
		public const string CreatedOn = "SystemCreatedOn";
	}
}
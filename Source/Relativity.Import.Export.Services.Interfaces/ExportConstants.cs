namespace Relativity.Import.Export.Services
{
	public static class ExportConstants
	{
		/// <summary>
		/// Used to pick out the COALESCED text field from export precedence
		/// </summary>
		/// <remarks>Spaces are here to ensure that there will never be a a sql column name conflict, because we strip all punctuation out of non-generated fields' column names for query results</remarks>
		///
		public const string TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME = "Text Precedence";
		public const string TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME = "KCURA FULL TEXT SOURCE";
		public const string TEXT_PRECEDENCE_AWARE_TEXT_SIZE = "KCURA FULL TEXT SIZE";
	}
}
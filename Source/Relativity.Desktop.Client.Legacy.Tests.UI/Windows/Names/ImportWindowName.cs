namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names
{
	internal sealed class ImportWindowName : RdcWindowName
	{
		public static readonly ImportWindowName ImportProductionLoadFile =
			new ImportWindowName("Relativity Desktop Client | Import Production Load File");

		public static readonly ImportWindowName ImportImageLoadFile =
			new ImportWindowName("Relativity Desktop Client | Import Image Load File");

		private ImportWindowName(string value) : base(value)
		{
		}
	}
}
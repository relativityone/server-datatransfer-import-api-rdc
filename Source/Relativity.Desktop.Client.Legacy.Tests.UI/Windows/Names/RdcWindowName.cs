using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names
{
	internal class RdcWindowName : WindowName
	{
		public static readonly RdcWindowName RelativityDesktopClient = new RdcWindowName("Relativity Desktop Client");
		public static readonly RdcWindowName RelativityLogin = new RdcWindowName("Relativity Login");

		public static readonly RdcWindowName SelectWorkspace =
			new RdcWindowName("Relativity Desktop Client | Select Workspace");

		public static readonly RdcWindowName ImportDocumentLoadFile =
			new RdcWindowName("Relativity Desktop Client | Import Document Load File");

		public static readonly RdcWindowName UntrustedCertificate = new RdcWindowName("Untrusted Certificate");

		protected RdcWindowName(string value) : base(value)
		{
		}
	}
}
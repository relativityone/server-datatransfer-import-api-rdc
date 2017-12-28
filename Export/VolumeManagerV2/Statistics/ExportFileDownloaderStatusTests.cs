using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	[TestFixture]
	public class ExportFileDownloaderStatusTests
	{
		private ExportFileDownloaderStatus _instance;

		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = new ExportFileDownloaderStatus(new NullLogger());
		}

		[Test]
		[TestCase("Aspera", FileDownloader.FileAccessType.Aspera)]
		[TestCase("aspera", FileDownloader.FileAccessType.Aspera)]
		[TestCase("Direct", FileDownloader.FileAccessType.Direct)]
		[TestCase("direct", FileDownloader.FileAccessType.Direct)]
		[TestCase("Web", FileDownloader.FileAccessType.Web)]
		[TestCase("web", FileDownloader.FileAccessType.Web)]
		[TestCase("Initializing", FileDownloader.FileAccessType.Initializing)]
		[TestCase("initializing", FileDownloader.FileAccessType.Initializing)]
		[TestCase("", FileDownloader.FileAccessType.Initializing)]
		[TestCase("Unknown", FileDownloader.FileAccessType.Initializing)]
		public void ItShouldUpdateUploaderType(string clientName, FileDownloader.FileAccessType type)
		{
			string mode = string.Empty;

			_instance.Attach(_tapiBridge.Object);
			_instance.UploadModeChangeEvent += s => mode = s;

			//ACT
			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(clientName, 0));

			//ASSERT
			Assert.That(mode, Is.EqualTo(type.ToString()));
			Assert.That(_instance.UploaderType, Is.EqualTo(type));
		}

		[Test]
		public void ItShouldDetach()
		{
			const string aspera = "Aspera";
			const string web = "Web";
			string mode = string.Empty;

			_instance.Attach(_tapiBridge.Object);
			_instance.UploadModeChangeEvent += s => mode = s;

			//ACT
			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(aspera, 0));

			_instance.Detach();

			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(web, 0));

			//ASSERT
			Assert.That(mode, Is.EqualTo(aspera));
			Assert.That(_instance.UploaderType, Is.EqualTo(FileDownloader.FileAccessType.Aspera));
		}
	}
}
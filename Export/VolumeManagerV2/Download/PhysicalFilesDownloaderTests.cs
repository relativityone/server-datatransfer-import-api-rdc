using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	[TestFixture]
	public class PhysicalFilesDownloaderTests
	{
		private Mock<IDownloadTapiBridge> _tapiBridge;
		private Mock<IExportTapiBridgeFactory> _exportTapiBridgeFactory;
		private ILog _logger;
		private SafeIncrement _safeIncrement;

		[SetUp]
		public void SetUp()
		{
			_tapiBridge = new Mock<IDownloadTapiBridge>();
			_exportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			_exportTapiBridgeFactory.Setup(f => f.CreateForFiles(It.IsAny<Credential>(), It.IsAny<CancellationToken>()))
				.Returns(() => _tapiBridge.Object);
			_logger = new NullLogger();
			_safeIncrement = new SafeIncrement();
		}

		[Test]
		public async Task ItShouldCreateTapiBridgesAccordinglyAndGroupFilesCorrectly()
		{
			string[] fileshares = {@"\\fileshare.one", @"\\fileshare.two", @"\\fileshare.three" };
			var asperaCredentialsService = new AsperaCredentialsServiceStub(fileshares);

			List<ExportRequest> requests = CreateThreeExportRequestsPerFileshares(fileshares).ToList();
			
			var downloader = new PhysicalFilesDownloader(asperaCredentialsService, _exportTapiBridgeFactory.Object, _safeIncrement, _logger);

			await downloader.DownloadFilesAsync(requests, CancellationToken.None);

			_exportTapiBridgeFactory.Verify(f => f.CreateForFiles(It.IsAny<Credential>(), It.IsAny<CancellationToken>()), Times.Exactly(fileshares.Length));
		}

		private IEnumerable<ExportRequest> CreateThreeExportRequestsPerFileshares(IEnumerable<string> fileshares)
		{
			const int filesPerFileshare = 3;
			return fileshares.SelectMany(f => Enumerable.Repeat(f, filesPerFileshare)).Select(CreatePhysicalFileExportRequest);
		}

		private PhysicalFileExportRequest CreatePhysicalFileExportRequest(string fileshareAddress)
		{
			return new PhysicalFileExportRequest(
				new ObjectExportInfo
				{
					NativeSourceLocation = new UriBuilder(fileshareAddress) {Path = Guid.NewGuid().ToString()}.ToString()
				}, "whatever.txt");
		}
	}

	public class AsperaCredentialsServiceStub : IAsperaCredentialsService
	{
		private readonly Dictionary<Uri, Credential> _fileshares;

		public AsperaCredentialsServiceStub(IEnumerable<string> fileshares)
		{
			_fileshares = fileshares.ToDictionary(f => new Uri(f), _ => new Credential());
		}

		public Credential GetAsperaCredentialsForFileshare(Uri fileUri)
		{
			return _fileshares.FirstOrDefault(kvp => kvp.Key.IsBaseOf(fileUri)).Value;
		}
	}
}

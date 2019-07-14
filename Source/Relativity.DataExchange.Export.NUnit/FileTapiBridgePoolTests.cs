// -----------------------------------------------------------------------------------------------------
// <copyright file="FileTapiBridgePoolTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Net;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	[TestFixture]
	public class FileTapiBridgePoolTests
	{
		private const int _DOCUMENT_ARTIFACT_TYPE = 10;

		private FileTapiBridgePool _uut;

		// UUT dependencies
		private Mock<IExportConfig> _exportConfig;

		private TapiBridgeParametersFactory _tapiBridgeParametersFactory;

		private DownloadProgressManager _downloadProgressManager;

		private FilesStatistics _filesStatistics;

		private Mock<IMessagesHandler> _messageHandler;

		private Mock<ITransferClientHandler> _transferClientHandler;

		private Mock<ILog> _logger;

		// Indirect dependencies
		private ExportFile _exportFile;

		private NativeRepository _nativeRepository;

		private ImageRepository _imageRepository;

		private LongTextRepository _longTextRepository;

		private Mock<IFile> _fileHelper;

		private Mock<IStatus> _status;

		private kCura.WinEDDS.Statistics _statistics;

		private Mock<ITapiObjectService> _tapiObjectService;

		[SetUp]
		public void Setup()
		{
			this._exportConfig = new Mock<IExportConfig>();
			this._exportFile = new ExportFile(_DOCUMENT_ARTIFACT_TYPE);
			this._exportFile.CaseInfo = new CaseInfo { ArtifactID = 1 };
			this._exportFile.Credential = new NetworkCredential();
			this._tapiBridgeParametersFactory =
				new TapiBridgeParametersFactory(this._exportFile, this._exportConfig.Object);
			this._tapiObjectService = new Mock<ITapiObjectService>();
			this._nativeRepository = new NativeRepository();
			this._imageRepository = new ImageRepository();
			this._fileHelper = new Mock<IFile>();
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(this._fileHelper.Object, this._logger.Object);
			this._status = new Mock<IStatus>();
			this._downloadProgressManager = new DownloadProgressManager(
				this._nativeRepository,
				this._imageRepository,
				this._longTextRepository,
				this._fileHelper.Object,
				this._status.Object,
				this._logger.Object);

			this._statistics = new kCura.WinEDDS.Statistics();
			this._filesStatistics = new FilesStatistics(this._statistics, this._fileHelper.Object, this._logger.Object);

			this._messageHandler = new Mock<IMessagesHandler>();

			this._transferClientHandler = new Mock<ITransferClientHandler>();

			this._uut = new FileTapiBridgePool(
				this._tapiBridgeParametersFactory,
				this._tapiObjectService.Object,
				this._downloadProgressManager,
				this._filesStatistics,
				this._messageHandler.Object,
				this._transferClientHandler.Object,
				this._logger.Object);
		}

		[Test]
		public void ItShouldRequestTheDownloadTapiBridge()
		{
			Assert.That(this._uut.Count, Is.EqualTo(0));
			Mock<IRelativityFileShareSettings> settings1 = CreateMockRelativityFileShareSettings(1, @"\\files1\Files");
			IDownloadTapiBridge bridge1 = this._uut.Request(settings1.Object, CancellationToken.None);
			Assert.That(bridge1, Is.Not.Null);
			Assert.That(this._uut.Count, Is.EqualTo(1));

			Mock<IRelativityFileShareSettings> settings2 = CreateMockRelativityFileShareSettings(2, @"\\files2\Files");
			IDownloadTapiBridge bridge2 = this._uut.Request(settings2.Object, CancellationToken.None);
			Assert.That(bridge2, Is.Not.Null);
			Assert.That(this._uut.Count, Is.EqualTo(2));
			Assert.That(bridge1, Is.Not.SameAs(bridge2));

			IDownloadTapiBridge bridge3 = this._uut.Request(settings1.Object, CancellationToken.None);
			Assert.That(bridge3, Is.Not.Null);
			Assert.That(this._uut.Count, Is.EqualTo(2));
			Assert.That(bridge3, Is.SameAs(bridge1));

			IDownloadTapiBridge bridge4 = this._uut.Request(settings2.Object, CancellationToken.None);
			Assert.That(bridge4, Is.Not.Null);
			Assert.That(this._uut.Count, Is.EqualTo(2));
			Assert.That(bridge4, Is.SameAs(bridge2));
		}

		[Test]
		public void ItShouldAssignTheUnmappedFileRepositoryClientsOnBridgeWhenTheSettingsIsNull()
		{
			this._tapiObjectService.Setup(x => x.GetUnmappedFileRepositoryClients()).Returns("Whatever");
			IDownloadTapiBridge bridge = this._uut.Request(null, CancellationToken.None);
			Assert.IsNotNull(bridge);

			// This represents the key parameter that's assigned to exclude Aspera mode.
			Assert.That(bridge.Parameters.ForceClientCandidates, Is.EqualTo("Whatever"));
		}

		[Test]
		public void ItShouldDisposeAllTapiBridges()
		{
			Mock<IRelativityFileShareSettings> settings1 = CreateMockRelativityFileShareSettings(1, @"\\files1\Files");
			Mock<IRelativityFileShareSettings> settings2 = CreateMockRelativityFileShareSettings(1, @"\\files2\Files");
			this._uut.Request(settings1.Object, CancellationToken.None);
			this._uut.Request(settings2.Object, CancellationToken.None);
			Assert.That(this._uut.Count, Is.EqualTo(2));
			this._uut.Dispose();
			Assert.That(this._uut.Count, Is.Zero);
		}

		private static Mock<IRelativityFileShareSettings> CreateMockRelativityFileShareSettings(int artifactId, string path)
		{
			Mock<IRelativityFileShareSettings> settings = new Mock<IRelativityFileShareSettings>();
			settings.SetupGet(x => x.ArtifactId).Returns(artifactId);
			settings.SetupGet(x => x.UncPath).Returns(path);
			return settings;
		}
	}
}
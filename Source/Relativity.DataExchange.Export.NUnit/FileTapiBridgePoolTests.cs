// -----------------------------------------------------------------------------------------------------
// <copyright file="FileTapiBridgePoolTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Io;
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

		[SetUp]
		public void Setup()
		{
			this._exportConfig = new Mock<IExportConfig>();

			this._exportFile = new ExportFile(_DOCUMENT_ARTIFACT_TYPE);
			this._tapiBridgeParametersFactory = new TapiBridgeParametersFactory(this._exportFile, this._exportConfig.Object);

			this._nativeRepository = new NativeRepository();
			this._imageRepository = new ImageRepository();
			this._fileHelper = new Mock<IFile>();
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(this._fileHelper.Object, this._logger.Object);
			this._status = new Mock<IStatus>();
			this._downloadProgressManager = new DownloadProgressManager(this._nativeRepository, this._imageRepository, this._longTextRepository, this._fileHelper.Object, this._status.Object, this._logger.Object);

			this._statistics = new kCura.WinEDDS.Statistics();
			this._filesStatistics = new FilesStatistics(this._statistics, this._fileHelper.Object, this._logger.Object);

			this._messageHandler = new Mock<IMessagesHandler>();

			this._transferClientHandler = new Mock<ITransferClientHandler>();

			this._uut = new FileTapiBridgePool(this._exportConfig.Object, this._tapiBridgeParametersFactory, this._downloadProgressManager, this._filesStatistics, this._messageHandler.Object, this._transferClientHandler.Object, this._logger.Object);
		}

		[Test]
		public void ItShouldReturnBridgeWithNullSettings()
		{
			IDownloadTapiBridge bridge = null;
			Assert.DoesNotThrow(() => bridge = this._uut.Request(null, CancellationToken.None));
			Assert.IsNotNull(bridge);
		}

		[Test]
		public void ItShouldNotThrowWhenReleasingNullSettingsBridge()
		{
			IDownloadTapiBridge bridge = this._uut.Request(null, CancellationToken.None);
			Assert.DoesNotThrow(() => this._uut.Release(bridge));
		}
	}
}

// -----------------------------------------------------------------------------------------------------
// <copyright file="FileTapiBridgePoolTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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
			_exportConfig = new Mock<IExportConfig>();

			_exportFile = new ExportFile(_DOCUMENT_ARTIFACT_TYPE);
			_tapiBridgeParametersFactory = new TapiBridgeParametersFactory(_exportFile, _exportConfig.Object);

			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_fileHelper = new Mock<IFile>();
			_logger = new Mock<ILog>();
			_longTextRepository = new LongTextRepository(_fileHelper.Object, _logger.Object);
			_status = new Mock<IStatus>();
			_downloadProgressManager = new DownloadProgressManager(_nativeRepository, _imageRepository, _longTextRepository, _fileHelper.Object, _status.Object, _logger.Object);

			_statistics = new kCura.WinEDDS.Statistics();
			_filesStatistics = new FilesStatistics(_statistics, _fileHelper.Object, _logger.Object);

			_messageHandler = new Mock<IMessagesHandler>();

			_transferClientHandler = new Mock<ITransferClientHandler>();

			_uut = new FileTapiBridgePool(_exportConfig.Object, _tapiBridgeParametersFactory, _downloadProgressManager, _filesStatistics, _messageHandler.Object, _transferClientHandler.Object, _logger.Object);
		}

		[Test]
		public void ItShouldReturnBridgeWithNullSettings()
		{
			IDownloadTapiBridge bridge = null;
			Assert.DoesNotThrow(() => bridge = _uut.Request(null, CancellationToken.None));
			Assert.IsNotNull(bridge);
		}

		[Test]
		public void ItShouldNotThrowWhenReleasingNullSettingsBridge()
		{
			IDownloadTapiBridge bridge = _uut.Request(null, CancellationToken.None);
			Assert.DoesNotThrow(() => _uut.Release(bridge));
		}
	}
}

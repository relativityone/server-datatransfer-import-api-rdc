using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download.TapiHelpers
{
	[TestFixture]
	public class FileTapiBridgePoolTests
	{
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
		private Mock<IFileHelper> _fileHelper;
		private Mock<IStatus> _status;
		private WinEDDS.Statistics _statistics;

		private const int _DOCUMENT_ARTIFACT_TYPE = 10;

		[SetUp]
		public void Setup()
		{
			_exportConfig = new Mock<IExportConfig>();

			_exportFile = new ExportFile(_DOCUMENT_ARTIFACT_TYPE);
			_tapiBridgeParametersFactory = new TapiBridgeParametersFactory(_exportFile, _exportConfig.Object);

			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_fileHelper = new Mock<IFileHelper>();
			_logger = new Mock<ILog>();
			_longTextRepository = new LongTextRepository(_fileHelper.Object, _logger.Object);
			_status = new Mock<IStatus>();
			_downloadProgressManager = new DownloadProgressManager(_nativeRepository, _imageRepository, _longTextRepository, _fileHelper.Object, _status.Object, _logger.Object);

			_statistics = new WinEDDS.Statistics();
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

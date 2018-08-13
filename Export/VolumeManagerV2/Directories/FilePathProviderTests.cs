using System.IO;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public abstract class FilePathProviderTests
	{
		private Mock<IDirectoryHelper> _directoryHelper;
		private Mock<ILabelManagerForArtifact> _labelManager;

		private ExportFile _exportSettings;

		private FilePathProvider _instance;

		private readonly string _volumeLabel = "volume_label";
		private readonly string _folderPath = "folder_path";

		[SetUp]
		public void SetUp()
		{
			_directoryHelper = new Mock<IDirectoryHelper>();

			_labelManager = new Mock<ILabelManagerForArtifact>();
			_labelManager.Setup(x => x.GetVolumeLabel(It.IsAny<int>())).Returns(_volumeLabel);
			_labelManager.Setup(x => x.GetImageSubdirectoryLabel(It.IsAny<int>())).Returns(Subdirectory);
			_labelManager.Setup(x => x.GetNativeSubdirectoryLabel(It.IsAny<int>())).Returns(Subdirectory);
			_labelManager.Setup(x => x.GetTextSubdirectoryLabel(It.IsAny<int>())).Returns(Subdirectory);

			_exportSettings = new ExportFile(1)
			{
				FolderPath = _folderPath
			};

			_instance = CreateInstance(_directoryHelper.Object, _labelManager.Object, _exportSettings);
		}

		protected abstract FilePathProvider CreateInstance(IDirectoryHelper directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings);

		protected abstract string Subdirectory { get; }

		[Test]
		public void ItShouldCreateDirectoryIfNotExists()
		{
			const string fileName = "file_name.txt";

			_directoryHelper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

			//ACT
			_instance.GetPathForFile(fileName, 0);

			//ASSERT
			string expectedPath = Path.Combine(_folderPath, _volumeLabel, Subdirectory);
			_directoryHelper.Verify(x => x.CreateDirectory(expectedPath), Times.Once);
		}

		[Test]
		public void ItShouldReturnValidPath()
		{
			const string fileName = "file_name.txt";

			//ACT
			string result = _instance.GetPathForFile(fileName, 0);

			//ASSERT
			string expectedFilePath = Path.Combine(_folderPath, _volumeLabel, Subdirectory, fileName);
			Assert.That(result, Is.EqualTo(expectedFilePath));
		}
	}
}
// -----------------------------------------------------------------------------------------------------
// <copyright file="FilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.IO;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

    using Moq;

    using Relativity.Import.Export.Io;

    [TestFixture]
	public abstract class FilePathProviderTests
	{
		private readonly string _volumeLabel = "volume_label";
		private readonly string _folderPath = "folder_path";
		private Mock<IDirectory> _directoryHelper;
		private Mock<ILabelManagerForArtifact> _labelManager;
		private ExportFile _exportSettings;
		private FilePathProvider _instance;

		protected abstract string Subdirectory { get; }

		[SetUp]
		public void SetUp()
		{
			_directoryHelper = new Mock<IDirectory>();

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

		protected abstract FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings);

		[Test]
		public void ItShouldCreateDirectoryIfNotExists()
		{
			const string fileName = "file_name.txt";

			_directoryHelper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

			// ACT
			_instance.GetPathForFile(fileName, 0);

			// ASSERT
			string expectedPath = Path.Combine(_folderPath, _volumeLabel, Subdirectory);
			_directoryHelper.Verify(x => x.CreateDirectory(expectedPath), Times.Once);
		}

		[Test]
		public void ItShouldReturnValidPath()
		{
			const string fileName = "file_name.txt";

			// ACT
			string result = _instance.GetPathForFile(fileName, 0);

			// ASSERT
			string expectedFilePath = Path.Combine(_folderPath, _volumeLabel, Subdirectory, fileName);
			Assert.That(result, Is.EqualTo(expectedFilePath));
		}
	}
}
// -----------------------------------------------------------------------------------------------------
// <copyright file="FilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.IO;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Io;

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
			this._directoryHelper = new Mock<IDirectory>();

			this._labelManager = new Mock<ILabelManagerForArtifact>();
			this._labelManager.Setup(x => x.GetVolumeLabel(It.IsAny<int>())).Returns(this._volumeLabel);
			this._labelManager.Setup(x => x.GetImageSubdirectoryLabel(It.IsAny<int>())).Returns(this.Subdirectory);
			this._labelManager.Setup(x => x.GetNativeSubdirectoryLabel(It.IsAny<int>())).Returns(this.Subdirectory);
			this._labelManager.Setup(x => x.GetTextSubdirectoryLabel(It.IsAny<int>())).Returns(this.Subdirectory);
			this._labelManager.Setup(x => x.GetPdfSubdirectoryLabel(It.IsAny<int>())).Returns(this.Subdirectory);

			this._exportSettings = new ExportFile(1)
			{
				FolderPath = this._folderPath
			};

			this._instance = this.CreateInstance(this._directoryHelper.Object, this._labelManager.Object, this._exportSettings);
		}

		protected abstract FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings);

		[Test]
		public void ItShouldCreateDirectoryIfNotExists()
		{
			const string fileName = "file_name.txt";

			this._directoryHelper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

			// ACT
			this._instance.GetPathForFile(fileName, 0);

			// ASSERT
			string expectedPath = Path.Combine(this._folderPath, this._volumeLabel, this.Subdirectory);
			this._directoryHelper.Verify(x => x.CreateDirectory(expectedPath), Times.Once);
		}

		[Test]
		public void ItShouldReturnValidPath()
		{
			const string fileName = "file_name.txt";

			// ACT
			string result = this._instance.GetPathForFile(fileName, 0);

			// ASSERT
			string expectedFilePath = Path.Combine(this._folderPath, this._volumeLabel, this.Subdirectory, fileName);
			Assert.That(result, Is.EqualTo(expectedFilePath));
		}
	}
}
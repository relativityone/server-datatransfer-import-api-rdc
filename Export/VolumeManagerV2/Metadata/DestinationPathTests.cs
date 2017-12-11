using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata
{
	[TestFixture]
	public class DestinationPathTests
	{
		[Test]
		[TestCase("dat", "ABC\\DEF", "Prefix", ExpectedResult = "ABC\\DEF\\Prefix_export.dat")]
		[TestCase("csv", "C:\\Export\\", "", ExpectedResult = "C:\\Export\\_export.csv")]
		[TestCase("html", "", "Pref", ExpectedResult = "Pref_export.html")]
		[TestCase("abc", "\\Q\\", "AbC", ExpectedResult = "\\Q\\AbC_export.abc")]
		[TestCase(null, "", "", ExpectedResult = "_export.")]
		public string ItShouldReturnValidLoadFilePath(string extension, string folderPath, string prefix)
		{
			var exportSettings = new ExportFile(1)
			{
				LoadFileExtension = extension,
				FolderPath = folderPath,
				LoadFilesPrefix = prefix
			};

			var instance = new LoadFileDestinationPath(exportSettings);

			//ACT
			return instance.Path;
		}

		[Test]
		[TestCase(LoadFileType.FileFormat.IPRO, "ABC\\DEF", "Prefix", ExpectedResult = "ABC\\DEF\\Prefix_export.lfp")]
		[TestCase(LoadFileType.FileFormat.IPRO_FullText, "C:\\Export\\", "", ExpectedResult = "C:\\Export\\_export_FULLTEXT_.lfp")]
		[TestCase(LoadFileType.FileFormat.Opticon, "", "Pref", ExpectedResult = "Pref_export.opt")]
		[TestCase(null, "\\Q\\", "AbC", ExpectedResult = "\\Q\\AbC_export")]
		public string ItShouldReturnValidImageLoadFilePath(LoadFileType.FileFormat? extension, string folderPath, string prefix)
		{
			var exportSettings = new ExportFile(1)
			{
				LogFileFormat = extension,
				FolderPath = folderPath,
				LoadFilesPrefix = prefix
			};

			var instance = new ImageLoadFileDestinationPath(exportSettings);

			//ACT
			return instance.Path;
		}
	}
}
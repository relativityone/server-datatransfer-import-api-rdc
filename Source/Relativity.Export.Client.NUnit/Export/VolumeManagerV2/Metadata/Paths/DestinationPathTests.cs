using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exceptions;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Paths
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
			Encoding encoding = Encoding.BigEndianUnicode;
			var exportSettings = new ExportFile(1)
			{
				LoadFileExtension = extension,
				FolderPath = folderPath,
				LoadFilesPrefix = prefix,
				LoadFileEncoding = encoding
			};

			var instance = new LoadFileDestinationPath(exportSettings);

			//ACT
			Assert.That(instance.DestinationFileType, Is.EqualTo(FileWriteException.DestinationFile.Load));
			Assert.That(instance.Encoding, Is.EqualTo(encoding));
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
			Assert.That(instance.DestinationFileType, Is.EqualTo(FileWriteException.DestinationFile.Image));
			return instance.Path;
		}

		[Test]
		public void ItShouldReturnDefaultEncodingForOpticon()
		{
			Encoding encoding = Encoding.ASCII;
			var exportSettings = new ExportFile(1)
			{
				LoadFileEncoding = encoding,
				ExportImages = true,
				LogFileFormat = LoadFileType.FileFormat.Opticon
			};

			var instance = new ImageLoadFileDestinationPath(exportSettings);

			//ACT
			Assert.That(instance.Encoding, Is.EqualTo(Encoding.Default));
		}

		[Test]
		[TestCase(LoadFileType.FileFormat.IPRO)]
		[TestCase(LoadFileType.FileFormat.IPRO_FullText)]
		public void ItShouldReturnUtf8EncodingForIPRO(LoadFileType.FileFormat fileFormat)
		{
			Encoding encoding = Encoding.ASCII;
			var exportSettings = new ExportFile(1)
			{
				LoadFileEncoding = encoding,
				ExportImages = true,
				LogFileFormat = fileFormat
			};

			var instance = new ImageLoadFileDestinationPath(exportSettings);

			//ACT
			Assert.That(instance.Encoding, Is.EqualTo(Encoding.UTF8));
		}

		[Test]
		[TestCase(LoadFileType.FileFormat.Opticon)]
		[TestCase(LoadFileType.FileFormat.IPRO)]
		[TestCase(LoadFileType.FileFormat.IPRO_FullText)]
		public void ItShouldReturnLoadFileEncodingWhenNotExportingImages(LoadFileType.FileFormat fileFormat)
		{
			Encoding encoding = Encoding.ASCII;
			var exportSettings = new ExportFile(1)
			{
				LoadFileEncoding = encoding,
				ExportImages = false,
				LogFileFormat = fileFormat
			};

			var instance = new ImageLoadFileDestinationPath(exportSettings);

			//ACT
			Assert.That(instance.Encoding, Is.EqualTo(encoding));
		}
	}
}
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Natives
{
	[TestFixture]
	public class LineNativeFilePathTests
	{
		private LineNativeFilePath _instance;

		private ExportFile _exportSettings;

		private Mock<IFilePathTransformer> _filePathTransformer;

		private const char _QUOTE_DELIMITER = 'Q';
		private const char _RECORD_DELIMITER = 'R';

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				RecordDelimiter = _RECORD_DELIMITER
			};
			_filePathTransformer = new Mock<IFilePathTransformer>();
			_instance = new LineNativeFilePath(new DelimitedCellFormatter(_exportSettings), _exportSettings, _filePathTransformer.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSkipLineEntryWhenNotExportingNatives()
		{
			_exportSettings.ExportNative = false;

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = "native_temp_location",
				NativeSourceLocation = "native_source_location"
			};

			//ACT
			_instance.AddNativeFilePath(loadFileEntry, artifact);

			//ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(string.Empty));
		}

		[Test]
		public void ItShouldUseNativeSourceLocationWhenNotCopyingNativeFiles()
		{
			const string nativeSourceLocation = "native_source_location";

			_exportSettings.ExportNative = true;
			_exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = false
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = "native_temp_location",
				NativeSourceLocation = nativeSourceLocation
			};

			//ACT
			_instance.AddNativeFilePath(loadFileEntry, artifact);

			//ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(nativeSourceLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
		}

		[Test]
		public void ItShouldUseNativeTempLocationWhenCopyingNativeFiles()
		{
			const string nativeTempLocation = "native_temp_location";
			const string transformedLocation = "transformed_location";

			_exportSettings.ExportNative = true;
			_exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = true
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = nativeTempLocation,
				NativeSourceLocation = "native_source_location"
			};

			_filePathTransformer.Setup(x => x.TransformPath(nativeTempLocation)).Returns(transformedLocation);

			//ACT
			_instance.AddNativeFilePath(loadFileEntry, artifact);

			//ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(transformedLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
		}

		[Test]
		public void ItShouldHandleEmptyLocation()
		{
			const string nativeTempLocation = "";

			_exportSettings.ExportNative = true;
			_exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = true
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = nativeTempLocation,
				NativeSourceLocation = "native_source_location"
			};

			//ACT
			_instance.AddNativeFilePath(loadFileEntry, artifact);

			//ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(nativeTempLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
			_filePathTransformer.Verify(x => x.TransformPath(It.IsAny<string>()), Times.Never);
		}
	}
}
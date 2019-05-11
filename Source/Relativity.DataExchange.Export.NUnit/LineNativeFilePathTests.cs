// -----------------------------------------------------------------------------------------------------
// <copyright file="LineNativeFilePathTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.Logging;

	[TestFixture]
	public class LineNativeFilePathTests
	{
		private const char _QUOTE_DELIMITER = 'Q';
		private const char _RECORD_DELIMITER = 'R';
		private LineNativeFilePath _instance;
		private ExportFile _exportSettings;
		private Mock<IFilePathTransformer> _filePathTransformer;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				RecordDelimiter = _RECORD_DELIMITER
			};
			this._filePathTransformer = new Mock<IFilePathTransformer>();
			this._instance = new LineNativeFilePath(new DelimitedCellFormatter(this._exportSettings), this._exportSettings, this._filePathTransformer.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSkipLineEntryWhenNotExportingNatives()
		{
			this._exportSettings.ExportNative = false;

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = "native_temp_location",
				NativeSourceLocation = "native_source_location"
			};

			// ACT
			this._instance.AddNativeFilePath(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(string.Empty));
		}

		[Test]
		public void ItShouldUseNativeSourceLocationWhenNotCopyingNativeFiles()
		{
			const string nativeSourceLocation = "native_source_location";

			this._exportSettings.ExportNative = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = false
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = "native_temp_location",
				NativeSourceLocation = nativeSourceLocation
			};

			// ACT
			this._instance.AddNativeFilePath(loadFileEntry, artifact);

			// ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(nativeSourceLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
		}

		[Test]
		public void ItShouldUseNativeTempLocationWhenCopyingNativeFiles()
		{
			const string nativeTempLocation = "native_temp_location";
			const string transformedLocation = "transformed_location";

			this._exportSettings.ExportNative = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = true
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = nativeTempLocation,
				NativeSourceLocation = "native_source_location"
			};

			this._filePathTransformer.Setup(x => x.TransformPath(nativeTempLocation)).Returns(transformedLocation);

			// ACT
			this._instance.AddNativeFilePath(loadFileEntry, artifact);

			// ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(transformedLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
		}

		[Test]
		public void ItShouldHandleEmptyLocation()
		{
			const string nativeTempLocation = "";

			this._exportSettings.ExportNative = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyNativeFilesFromRepository = true
			};

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeTempLocation = nativeTempLocation,
				NativeSourceLocation = "native_source_location"
			};

			// ACT
			this._instance.AddNativeFilePath(loadFileEntry, artifact);

			// ASSERT
			string expectedValue = LoadFileTestHelpers.FormatPathEntry(nativeTempLocation, _QUOTE_DELIMITER, _RECORD_DELIMITER);
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedValue));
			this._filePathTransformer.Verify(x => x.TransformPath(It.IsAny<string>()), Times.Never);
		}
	}
}
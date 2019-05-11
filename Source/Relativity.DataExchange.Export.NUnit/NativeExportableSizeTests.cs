// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeExportableSizeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.DataSize;

	[TestFixture]
	public class NativeExportableSizeTests
	{
		private const long _NATIVE_FILE_SIZE = 647109;
		private const long _NATIVE_FILE_COUNT = 771933;
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;
		private NativeExportableSize _instance;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			this._volumePredictions = new VolumePredictions
			{
				NativeFileCount = _NATIVE_FILE_COUNT,
				NativeFilesSize = _NATIVE_FILE_SIZE
			};

			this._instance = new NativeExportableSize(this._exportSettings);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingNatives()
		{
			this._exportSettings.ExportNative = false;
			this._exportSettings.VolumeInfo.CopyNativeFilesFromRepository = true;

			// ACT
			this._instance.CalculateNativesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.NativeFileCount, Is.Zero);
			Assert.That(this._volumePredictions.NativeFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotCopyingFiles()
		{
			this._exportSettings.ExportNative = true;
			this._exportSettings.VolumeInfo.CopyNativeFilesFromRepository = false;

			// ACT
			this._instance.CalculateNativesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.NativeFileCount, Is.Zero);
			Assert.That(this._volumePredictions.NativeFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldNotChangeSizeAndCountWhenExportingFiles()
		{
			this._exportSettings.ExportNative = true;
			this._exportSettings.VolumeInfo.CopyNativeFilesFromRepository = true;

			// ACT
			this._instance.CalculateNativesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.NativeFileCount, Is.EqualTo(_NATIVE_FILE_COUNT));
			Assert.That(this._volumePredictions.NativeFilesSize, Is.EqualTo(_NATIVE_FILE_SIZE));
		}
	}
}
﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeExportableSizeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.DataSize;

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
			_exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			_volumePredictions = new VolumePredictions
			{
				NativeFileCount = _NATIVE_FILE_COUNT,
				NativeFilesSize = _NATIVE_FILE_SIZE
			};

			_instance = new NativeExportableSize(_exportSettings);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingNatives()
		{
			_exportSettings.ExportNative = false;
			_exportSettings.VolumeInfo.CopyNativeFilesFromRepository = true;

			// ACT
			_instance.CalculateNativesSize(_volumePredictions);

			// ASSERT
			Assert.That(_volumePredictions.NativeFileCount, Is.Zero);
			Assert.That(_volumePredictions.NativeFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotCopyingFiles()
		{
			_exportSettings.ExportNative = true;
			_exportSettings.VolumeInfo.CopyNativeFilesFromRepository = false;

			// ACT
			_instance.CalculateNativesSize(_volumePredictions);

			// ASSERT
			Assert.That(_volumePredictions.NativeFileCount, Is.Zero);
			Assert.That(_volumePredictions.NativeFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldNotChangeSizeAndCountWhenExportingFiles()
		{
			_exportSettings.ExportNative = true;
			_exportSettings.VolumeInfo.CopyNativeFilesFromRepository = true;

			// ACT
			_instance.CalculateNativesSize(_volumePredictions);

			// ASSERT
			Assert.That(_volumePredictions.NativeFileCount, Is.EqualTo(_NATIVE_FILE_COUNT));
			Assert.That(_volumePredictions.NativeFilesSize, Is.EqualTo(_NATIVE_FILE_SIZE));
		}
	}
}
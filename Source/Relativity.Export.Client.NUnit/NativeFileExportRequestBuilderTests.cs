// ----------------------------------------------------------------------------
// <copyright file="NativeFileExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Collections.Generic;
    using System.Threading;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
    using kCura.WinEDDS.Exporters;

    using global::NUnit.Framework;

    using Relativity.Logging;

    public class NativeFileExportRequestBuilderTests : ExportRequestBuilderTests
	{
		protected override ExportRequestBuilder CreateInstance(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics)
		{
			return new NativeFileExportRequestBuilder(filePathProvider, fileNameProvider, exportFileValidator, fileProcessingStatistics, new NullLogger());
		}

		[Test]
		public void ItShouldSkipNativeWithEmptyGuid()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = string.Empty
			};

			//ACT
			IList<ExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}
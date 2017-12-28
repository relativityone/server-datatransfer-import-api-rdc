using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	public class NativeFileExportRequestBuilderTests : FileExportRequestBuilderTests
	{
		protected override FileExportRequestBuilder CreateInstance(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator exportFileValidator,
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
			IList<FileExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}
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
	public class FieldFileExportRequestBuilderTests : FileExportRequestBuilderTests
	{
		protected override FileExportRequestBuilder CreateInstance(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				FileField = new DocumentField("", 1, 1, 1, null, null, null, true, null, null, false)
			};
			return new FieldFileExportRequestBuilder(filePathProvider, fileNameProvider, exportFileValidator, fileProcessingStatistics, new NullLogger(),
				new FieldFileExportRequestFactory(exportSettings));
		}

		[Test]
		public void ItShouldSkipFieldFileWithMissingId()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				FileID = 0,
				NativeSourceLocation = "location"
			};

			//ACT
			IList<FileExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
		}

		[Test]
		public void ItShouldSkipFieldFileWithEmptyLocation()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				FileID = 1,
				NativeSourceLocation = string.Empty
			};

			//ACT
			IList<FileExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}
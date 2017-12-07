using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FieldFileExportRequestBuilder : FileExportRequestBuilder
	{
		private readonly FieldFileExportRequestFactory _requestFactory;

		public FieldFileExportRequestBuilder(NativeFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, ExportFileValidator validator, ILog logger,
			FieldFileExportRequestFactory requestFactory) : base(filePathProvider, fileNameProvider, validator, logger)
		{
			_requestFactory = requestFactory;
		}

		protected override FileExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return _requestFactory.Create(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return artifact.FileID > 0 || !string.IsNullOrWhiteSpace(artifact.NativeSourceLocation);
		}
	}
}
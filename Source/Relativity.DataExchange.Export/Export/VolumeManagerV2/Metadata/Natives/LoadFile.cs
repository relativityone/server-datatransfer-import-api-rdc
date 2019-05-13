namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public class LoadFile : ILoadFile
	{
		private readonly LoadFileMetadataBuilder _loadFileMetadataBuilder;
		private readonly ILoadFileWriter _loadFileWriter;

		public LoadFile(LoadFileMetadataBuilder loadFileMetadataBuilder, ILoadFileWriter loadFileWriter)
		{
			_loadFileMetadataBuilder = loadFileMetadataBuilder;
			_loadFileWriter = loadFileWriter;
		}

		public void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			IDictionary<int, ILoadFileEntry> loadFileEntries = _loadFileMetadataBuilder.AddLines(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_loadFileWriter.Write(loadFileEntries, artifacts, cancellationToken);
		}
	}
}
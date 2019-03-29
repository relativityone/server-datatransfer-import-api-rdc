namespace Relativity.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Batches;

	public interface IImageLoadFileWriter : IStateful, IDisposable
	{
		void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}
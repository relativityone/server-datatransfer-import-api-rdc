using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class EmptyImageLoadFileWriter : IImageLoadFileWriter
	{
		public void Dispose()
		{
		}

		public void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
		}
	}
}
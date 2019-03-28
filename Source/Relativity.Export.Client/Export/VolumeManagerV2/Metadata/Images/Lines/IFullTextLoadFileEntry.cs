using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public interface IFullTextLoadFileEntry : IDisposable
	{
		void WriteFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token);
	}
}
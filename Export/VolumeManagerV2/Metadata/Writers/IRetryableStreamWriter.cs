using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public interface IRetryableStreamWriter : IDisposable, IStateful
	{
		void WriteEntry(string loadFileEntry, CancellationToken token);

		void WriteChunk(string chunk, CancellationToken token);

		void FlushChunks(CancellationToken token);
	}
}
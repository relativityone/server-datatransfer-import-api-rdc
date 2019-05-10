namespace Relativity.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.Threading;

	using Relativity.Export.VolumeManagerV2.Batches;

	public interface IRetryableStreamWriter : IDisposable, IStateful
	{
		void WriteEntry(string loadFileEntry, CancellationToken token);

		void WriteChunk(string chunk, CancellationToken token);

		void FlushChunks(CancellationToken token);

		void InitializeFile(CancellationToken token);
	}
}
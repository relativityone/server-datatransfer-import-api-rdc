// -----------------------------------------------------------------------------------------------------
// <copyright file="WriterStub.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Text;
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

	public class WriterStub : IRetryableStreamWriter
	{
		private readonly StringBuilder _stringBuilder = new StringBuilder();

		public string Text => this._stringBuilder.ToString();

		public void Dispose()
		{
		}

		public void SaveState()
		{
		}

		public void RestoreLastState()
		{
		}

		public void WriteEntry(string loadFileEntry, CancellationToken token)
		{
			this._stringBuilder.Append(loadFileEntry);
		}

		public void WriteChunk(string chunk, CancellationToken token)
		{
			this._stringBuilder.Append(chunk);
		}

		public void FlushChunks(CancellationToken token)
		{
		}

		public void InitializeFile(CancellationToken token)
		{
		}
	}
}
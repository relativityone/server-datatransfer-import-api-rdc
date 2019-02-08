// ----------------------------------------------------------------------------
// <copyright file="WriterStub.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Text;
    using System.Threading;

    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;

    public class WriterStub : IRetryableStreamWriter
	{
		private readonly StringBuilder _stringBuilder = new StringBuilder();

		public string Text => _stringBuilder.ToString();

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
			_stringBuilder.Append(loadFileEntry);
		}

		public void WriteChunk(string chunk, CancellationToken token)
		{
			_stringBuilder.Append(chunk);
		}

		public void FlushChunks(CancellationToken token)
		{
		}

		public void InitializeFile(CancellationToken token)
		{
		}
	}
}
// ----------------------------------------------------------------------------
// <copyright file="RestartableFileStream.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Data
{
	using System.IO;

	/// <summary>
	/// Restartable FileStream.
	/// </summary>
	internal class RestartableFileStream : Stream
	{
		private readonly string filename;

		private FileStream stream;

		private long restartPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestartableFileStream"/> class.
		/// </summary>
		/// <param name="filename">Filename to open.</param>
		public RestartableFileStream(string filename)
		{
			this.filename = filename;
			this.stream = OpenFileStream(filename);
		}

		/// <inheritdoc />
		public override bool CanRead => stream.CanRead;

		/// <inheritdoc />
		public override bool CanSeek => stream.CanSeek;

		/// <inheritdoc />
		public override bool CanWrite => stream.CanWrite;

		/// <inheritdoc />
		public override long Length => stream.Length;

		/// <inheritdoc />
		public override long Position
		{
			get => stream.Position;
			set => stream.Position = value;
		}

		/// <summary>
		/// Restarts the stream.
		/// </summary>
		public void Restart()
		{
			stream.Dispose();

			stream = OpenFileStream(filename);
			stream.Seek(restartPosition, SeekOrigin.Begin);
		}

		/// <inheritdoc />
		public override void Flush()
		{
			stream.Flush();
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			// grab the position first in case there is a partial read.
			restartPosition = stream.Position;

			int toReturn = stream.Read(buffer, offset, count);
			restartPosition = stream.Position;
			return toReturn;
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				stream.Dispose();
			}

			base.Dispose(disposing);
		}

		private static FileStream OpenFileStream(string filename)
		{
			return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 32 * 1024, FileOptions.SequentialScan);
		}
	}
}
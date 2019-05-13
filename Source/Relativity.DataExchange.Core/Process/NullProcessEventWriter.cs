// ----------------------------------------------------------------------------
// <copyright file="NullProcessEventWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;

	/// <summary>
	/// Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IProcessEventWriter"/> is referenced but whose functionality isn't actually used or required. This class cannot be inherited.
	/// </summary>
	public sealed class NullProcessEventWriter : IProcessEventWriter
	{
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="NullProcessEventWriter"/> class.
		/// </summary>
		public NullProcessEventWriter()
		{
			this.disposed = false;
			this.File = string.Empty;
		}

		/// <inheritdoc />
		public string File
		{
			get;
		}

		/// <inheritdoc />
		public bool HasEvents => false;

		/// <inheritdoc />
		public void Close()
		{
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public void Save(string targetFile)
		{
		}

		/// <inheritdoc />
		public void Write(ProcessEventDto dto)
		{
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing)
			{
			}

			this.disposed = true;
		}
	}
}
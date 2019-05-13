// ----------------------------------------------------------------------------
// <copyright file="NullProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Threading;

	/// <summary>
	/// Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IProcessErrorWriter"/> is referenced but whose functionality isn't actually used or required. This class cannot be inherited.
	/// </summary>
	public sealed class NullProcessErrorWriter : IProcessErrorWriter
	{
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="NullProcessErrorWriter"/> class.
		/// </summary>
		public NullProcessErrorWriter()
		{
			this.disposed = false;
		}

		/// <inheritdoc />
		public bool HasErrors => false;

		/// <inheritdoc />
		public ProcessErrorReport BuildErrorReport(CancellationToken token)
		{
			return new ProcessErrorReport();
		}

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
		public void Write(string key, string description)
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
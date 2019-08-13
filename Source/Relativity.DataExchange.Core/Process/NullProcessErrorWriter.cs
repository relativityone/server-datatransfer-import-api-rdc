// ----------------------------------------------------------------------------
// <copyright file="NullProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System.Threading;

	/// <summary>
	/// Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IProcessErrorWriter"/> is referenced but whose functionality isn't actually used or required. This class cannot be inherited.
	/// </summary>
	public sealed class NullProcessErrorWriter : IProcessErrorWriter
	{
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
		}

		/// <inheritdoc />
		public void Write(string key, string description)
		{
		}
	}
}
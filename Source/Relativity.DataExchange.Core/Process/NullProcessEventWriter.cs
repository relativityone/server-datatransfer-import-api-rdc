// ----------------------------------------------------------------------------
// <copyright file="NullProcessEventWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	/// <summary>
	/// Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IProcessEventWriter"/> is referenced but whose functionality isn't actually used or required. This class cannot be inherited.
	/// </summary>
	public sealed class NullProcessEventWriter : IProcessEventWriter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NullProcessEventWriter"/> class.
		/// </summary>
		public NullProcessEventWriter()
		{
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
		}

		/// <inheritdoc />
		public void Save(string targetFile)
		{
		}

		/// <inheritdoc />
		public void Write(ProcessEventDto dto)
		{
		}
	}
}
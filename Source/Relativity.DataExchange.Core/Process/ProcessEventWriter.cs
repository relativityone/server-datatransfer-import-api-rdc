// ----------------------------------------------------------------------------
// <copyright file="ProcessEventWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Xml.Serialization;
	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents an abstract object that writes output events to a file. This class cannot be inherited.
	/// </summary>
	internal sealed class ProcessEventWriter : IProcessEventWriter
	{
		private readonly IFileSystem fileSystem;
		private readonly XmlSerializer serializer;
		private IStreamWriter streamWriter;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessEventWriter"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		public ProcessEventWriter(IFileSystem fileSystem)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			this.fileSystem = fileSystem;
			this.File = null;
			this.streamWriter = null;
			this.disposed = false;
			this.serializer = new XmlSerializer(typeof(ProcessEventDto));
		}

		/// <inheritdoc />
		public string File
		{
			get;
			private set;
		}

		/// <inheritdoc />
		public bool HasEvents => !string.IsNullOrEmpty(this.File);

		/// <inheritdoc />
		public void Close()
		{
			this.streamWriter?.Close();
			this.streamWriter = null;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <inheritdoc />
		public void Save(string targetFile)
		{
			if (string.IsNullOrEmpty(targetFile))
			{
				throw new ArgumentNullException(nameof(targetFile));
			}

			if (!string.IsNullOrEmpty(this.File) && this.fileSystem.File.Exists(this.File))
			{
				this.fileSystem.File.Move(this.File, targetFile);
			}
		}

		/// <inheritdoc />
		public void Write(ProcessEventDto dto)
		{
			if (dto == null)
			{
				throw new ArgumentNullException(nameof(dto));
			}

			if (string.IsNullOrEmpty(this.File))
			{
				this.File = System.IO.Path.Combine(
					System.IO.Path.GetTempPath(),
					$"RDC_AllEvents_{Guid.NewGuid()}.tmp");
			}

			if (this.streamWriter?.BaseStream == null)
			{
				this.streamWriter = this.fileSystem.CreateStreamWriter(this.File, false);
			}

			this.serializer.Serialize(this.streamWriter.BaseStream, dto);
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
				if (this.streamWriter != null)
				{
					// Duplicated to avoid CA warning.
					this.streamWriter.Close();
					this.streamWriter = null;
				}

				// Always delete that which you create!
				if (!string.IsNullOrEmpty(this.File) && this.fileSystem.File.Exists(this.File))
				{
					this.fileSystem.File.Delete(this.File);
				}
			}

			this.disposed = true;
		}
	}
}
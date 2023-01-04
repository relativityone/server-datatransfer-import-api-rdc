// <copyright file="ErrorMessageWriter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Io
{
	using System;
	using System.IO;

	/// <summary>
	/// Class that is specialized in writing error messages to the error message file.
	/// </summary>
	/// <typeparam name="T">The type to write.</typeparam>
	public sealed class ErrorMessageWriter<T> : IDisposable
		where T : IErrorArguments
	{
		private readonly object lockObject = new object();
		private Lazy<StreamWriter> stream;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessageWriter{T}"/> class.
		/// </summary>
		/// <param name="filePath">File location for the error messages.</param>
		public ErrorMessageWriter(string filePath)
		{
			lock (this.lockObject)
			{
				if (string.IsNullOrEmpty(filePath))
				{
					filePath =
						TempFileBuilder.TemporaryFileNameWithoutCreatingEmptyFile(TempFileConstants.ErrorsFileNameSuffix);
				}

				this.FilePath = filePath;

				this.SetStreamToInitialValue(filePath);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessageWriter{T}"/> class.
		/// </summary>
		public ErrorMessageWriter()
			: this(string.Empty)
		{
		}

		/// <summary>
		/// Gets the location this writer is writing to.
		/// </summary>
		public string FilePath { get; }

		/// <summary>
		/// Gets a value indicating whether this class created a file. There are 2 possibilities:
		/// - There is no file
		/// - There is a file, and there is something in it.
		/// </summary>
		public bool FileCreated { get; private set; }

		/// <inheritdoc/>
		public void Dispose()
		{
			lock (this.lockObject)
			{
				if (this.disposed)
				{
					return;
				}

				if (this.stream.IsValueCreated)
				{
					this.stream.Value?.Dispose();
					this.disposed = true;
				}
			}
		}

		/// <summary>
		/// Writes an error message to the error file.
		/// </summary>
		/// <param name="toWrite">The line to write.</param>
		public void WriteErrorMessage(T toWrite)
		{
			lock (this.lockObject)
			{
				var lineForFile = toWrite.FormattedLineInFile();
				this.stream.Value.WriteLine(lineForFile);

				// We flush so we can never have half written lines in our file, for safety and convenience.
				this.stream.Value.Flush();
			}
		}

		/// <summary>
		/// Releases the (file) hold of the document until a message is written.
		/// </summary>
		internal void ReleaseLock()
		{
			lock (this.lockObject)
			{
				this.Dispose();
				this.SetStreamToInitialValue(this.FilePath);
				this.disposed = false;
			}
		}

		private void SetStreamToInitialValue(string filePath)
		{
			this.stream = new Lazy<StreamWriter>(() =>
				{
					this.FileCreated = true;
					return new StreamWriter(filePath, true, System.Text.Encoding.Default);
				});
		}
	}
}

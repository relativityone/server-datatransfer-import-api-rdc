﻿// ----------------------------------------------------------------------------
// <copyright file="NullIoReporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IIoReporter"/> is used wherever the object is <see langword="null" />.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Represents a <see langword="null" /> design pattern for occasions where a valid <see cref="IIoReporter"/> is used wherever the object is <see langword="null" />.
	/// </summary>
	internal class NullIoReporter : IIoReporter
	{
		private readonly IFileSystem fileSystem;

		/// <summary>
		/// Initializes a new instance of the <see cref="NullIoReporter"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		public NullIoReporter(IFileSystem fileSystem)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			this.fileSystem = fileSystem;
		}

		/// <inheritdoc />
		public void CopyFile(string sourceFileName, string destFileName, bool overwrite, int lineNumber)
		{
			this.fileSystem.File.Copy(sourceFileName, destFileName, overwrite);
		}

		/// <inheritdoc />
		public bool GetFileExists(string fileName, int lineNumber)
		{
			IFileInfo fileInfo = this.fileSystem.CreateFileInfo(fileName);
			bool fileExists = fileInfo.Exists;
			return fileExists;
		}

		/// <inheritdoc />
		public long GetFileLength(string fileName, int lineNumber)
		{
			IFileInfo fileInfo = this.fileSystem.CreateFileInfo(fileName);

			// We want any exceptions that occur when accessing properties to get thrown.
			long fileLength = fileInfo.Length;
			return fileLength;
		}

		/// <inheritdoc />
		public void PublishRetryMessage(
			Exception exception,
			TimeSpan timeSpan,
			int retryCount,
			int totalRetryCount,
			long lineNumber)
		{
		}

		/// <inheritdoc />
		public void PublishWarningMessage(IoWarningEventArgs args)
		{
		}
	}
}
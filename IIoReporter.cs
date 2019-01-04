using System;

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// Represents an object used to perform I/O operations, publish warning messages, and retry the operation.
	/// </summary>
	public interface IIoReporter
	{
		/// <summary>
		/// Property to expose IoWarningPublisher
		/// </summary>
		IoWarningPublisher IOWarningPublisher
		{
			get;
		}

		/// <summary>
		/// Copies an existing file to a new file. Overwriting a file of the same name is allowed.
		/// </summary>
		/// <param name="sourceFileName">
		/// The file to copy.
		/// </param>
		/// <param name="destFileName">
		/// The name of the destination file. This cannot be a directory.
		/// </param>
		/// <param name="overwrite">
		/// <see langword="true" /> if the destination file can be overwritten; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="lineNumber">
		/// The current line number.
		/// </param>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission. -or-
		/// <paramref name="destFileName" /> is read-only.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> specifies a directory.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// <paramref name="sourceFileName" /> was not found.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// <paramref name="destFileName" /> exists and <paramref name="overwrite" /> is <see langword="false" />.-or- An I/O error has occurred.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
		/// </exception>
		void CopyFile(string sourceFileName, string destFileName, bool overwrite, int lineNumber);

		/// <summary>
		/// Gets a value indicating whether the file exists. When an I/O error occurs, warning messages are published and the operation is retried.
		/// </summary>
		/// <param name="fileName">
		/// The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator character.
		/// </param>
		/// <param name="lineNumber">
		/// The current line number.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the file exists; <see langword="false" /> if the file does not exist or if the file is a directory.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot update the state of the file or directory.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The file does not exist.-or- The <see langword="Length" /> property is called for a directory.
		/// </exception>
		bool GetFileExists(string fileName, int lineNumber);

		/// <summary>
		/// Gets the size, in bytes, of the current file. When an I/O error occurs, warning messages are published and the operation is retried.
		/// </summary>
		/// <param name="fileName">
		/// The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator character.
		/// </param>
		/// <param name="lineNumber">
		/// The current line number.
		/// </param>
		/// <returns>
		/// The size of the current file in bytes.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot update the state of the file or directory.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The file does not exist.-or- The <see langword="Length" /> property is called for a directory.
		/// </exception>
		long GetFileLength(string fileName, int lineNumber);

		/// <summary>
		/// Publishes a retry-based warning message and logs the exception.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="timeSpan">
		/// The time span between retry attempts.
		/// </param>
		/// <param name="retryCount">
		/// The current retry count.
		/// </param>
		/// <param name="totalRetryCount">
		/// The total retry count.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		void PublishRetryMessage(Exception exception, TimeSpan timeSpan, int retryCount, int totalRetryCount,
			long lineNumber);
	}
}
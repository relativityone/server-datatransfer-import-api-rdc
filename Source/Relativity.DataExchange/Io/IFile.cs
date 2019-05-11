// ----------------------------------------------------------------------------
// <copyright file="IFile.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents an abstract wrapper for the <see cref="T:System.IO.File"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IFile
	{
		/// <summary>
		/// Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
		/// </summary>
		/// <param name="sourceFileName">
		/// The file to copy.
		/// </param>
		/// <param name="destFileName">
		/// The name of the destination file. This cannot be a directory or an existing file.
		/// </param>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or-
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> specifies a directory.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// <paramref name="sourceFileName" /> was not found.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// <paramref name="destFileName" /> exists.-or- An I/O error has occurred.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
		/// </exception>
		void Copy(string sourceFileName, string destFileName);

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
		void Copy(string sourceFileName, string destFileName, bool overwrite);

		/// <summary>
		/// Counts the number of lines in the file specified by the provided path.
		/// </summary>
		/// <param name="path">
		/// The path to count lines.
		/// </param>
		/// <returns>
		/// The number of lines in the file.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// The <paramref name="path"/> is empty, contains only white spaces, or contains invalid characters.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="path"/> is null.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The <paramref name="path"/> does not exist.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <paramref name="path"/> is in an invalid format.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// Access to <paramref name="path"/> is denied.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The <paramref name="path"/> is too long (greater than 260 characters).
		/// </exception>
		int CountLinesInFile(string path);

		/// <summary>
		/// Creates or overwrites a file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.IO.FileStream" /> that provides read/write access to the file specified in <paramref name="path" />.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.-or-
		/// <paramref name="path" /> specified a file that is read-only.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurred while creating the file.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format.
		/// </exception>
		System.IO.FileStream Create(string path);

		/// <summary>
		/// Creates or overwrites a file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <param name="append">
		/// <see langword="true" /> to append data to this file; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.IO.FileStream" /> that provides read/write access to the file specified in <paramref name="path" />.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.-or-
		/// <paramref name="path" /> specified a file that is read-only.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurred while creating the file.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format.
		/// </exception>
		System.IO.FileStream Create(string path, bool append);

		/// <summary>
		/// Creates or overwrites a file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <param name="mode">
		/// A constant that determines how to open or create the file.
		/// </param>
		/// <param name="access">
		/// A constant that determines how the file can be accessed by the <see langword="FileStream" /> object. This also determines the values returned by the <see cref="P:System.IO.FileStream.CanRead" /> and <see cref="P:System.IO.FileStream.CanWrite" /> properties of the <see langword="FileStream" /> object. <see cref="P:System.IO.FileStream.CanSeek" /> is <see langword="true" /> if <paramref name="path" /> specifies a disk file.
		/// </param>
		/// <param name="share">
		/// A constant that determines how the file will be shared by processes.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.IO.FileStream" /> that provides read/write access to the file specified in <paramref name="path" />.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.-or-
		/// <paramref name="path" /> specified a file that is read-only.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurred while creating the file.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format.
		/// </exception>
		System.IO.FileStream Create(
			string path,
			System.IO.FileMode mode,
			System.IO.FileAccess access,
			System.IO.FileShare share);

		/// <summary>
		/// Creates or opens a file for writing UTF-8 encoded text.
		/// </summary>
		/// <param name="path">
		/// The file to be opened for writing.
		/// </param>
		/// <returns>
		/// A <see cref="T:System.IO.StreamWriter" /> that writes to the specified file using UTF-8 encoding.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format.
		/// </exception>
		System.IO.StreamWriter CreateText(string path);

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="path">
		/// The name of the file to be deleted. Wildcard characters are not supported.
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// The specified file is in use. -or-There is an open handle on the file, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.-or- The file is an executable file that is in use.-or-
		/// <paramref name="path" /> is a directory.-or-
		/// <paramref name="path" /> specified a read-only file.
		/// </exception>
		void Delete(string path);

		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="path">
		/// The file to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the caller has the required permissions and path contains the name of an existing file; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if path is a null reference (Nothing in Visual Basic), an invalid path, or a zero-length string. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns false regardless of the existence of path.
		/// </returns>
		bool Exists(string path);

		/// <summary>
		/// Gets the size of the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// The size of the file in bytes.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// A serious I/O error occurred attempting to access the file.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The file does not exist.
		/// </exception>
		long GetFileSize(string fileName);

		/// <summary>
		/// Moves a specified file to a new location, providing the option to specify a new file name.
		/// </summary>
		/// <param name="sourceFileName">
		/// The name of the file to move. Can include a relative or absolute path.
		/// </param>
		/// <param name="destFileName">
		/// The new path and name for the file.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// The destination file already exists.-or-
		/// <paramref name="sourceFileName" /> was not found.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains invalid characters as defined in <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is invalid, (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format.
		/// </exception>
		void Move(string sourceFileName, string destFileName);

		/// <summary>
		/// Opens a text file, reads all lines of the file, and then closes the file.
		/// </summary>
		/// <param name="path">
		/// The file to open for reading.
		/// </param>
		/// <returns>
		/// A string containing all lines of the file.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurred while opening the file.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// <paramref name="path" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or-
		/// <paramref name="path" /> specified a directory.-or- The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The file specified in <paramref name="path" /> was not found.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> is in an invalid format. </exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.
		/// </exception>
		string ReadAllText(string path);

		/// <summary>
		/// Reopens a file stream and truncates the stream to the specified length.
		/// </summary>
		/// <param name="fileName">
		/// The name of the file to reopen.
		/// </param>
		/// <param name="length">
		/// The new length of the stream.
		/// </param>
		/// <returns>
		/// The <see cref="System.IO.FileStream"/> instance.
		/// </returns>
		System.IO.FileStream ReopenAndTruncate(string fileName, long length);
	}
}
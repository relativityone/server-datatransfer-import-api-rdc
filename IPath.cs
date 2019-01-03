// ----------------------------------------------------------------------------
// <copyright file="IPath.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a wrapper for <see cref="T:System.IO.Path"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IPath
	{
		/// <summary>
		/// Gets or sets a value indicating whether to support long paths. This is always <see langword="false" /> by default and must be explicitly enabled.
		/// </summary>
		/// <value>
		/// /// <see langword="true" /> to support long paths; otherwise, <see langword="false" />.
		/// </value>
		bool SupportLongPaths
		{
			get;
			set;
		}

		/// <summary>
		/// Adds a trailing back slash to <paramref name="path"/> if one isn't found.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// The modified path.
		/// </returns>
		string AddTrailingBackSlash(string path);

		/// <summary>
		/// Changes the extension of a path string.
		/// </summary>
		/// <param name="path">
		/// The path information to modify. The path cannot contain any of the characters defined in GetInvalidPathChars.
		/// </param>
		/// <param name="extension">
		/// The new extension (with or without a leading period). Specify null to remove an existing extension from path.
		/// </param>
		/// <returns>
		/// The modified path information. On Windows-based desktop platforms, if <paramref name="path" /> is <see langword="null" /> or an empty string (""), the path information is returned unmodified. If <paramref name="extension" /> is <see langword="null" />, the returned string contains the specified path with its extension removed. If <paramref name="path" /> has no extension, and <paramref name="extension" /> is not <see langword="null" />, the returned path string contains <paramref name="extension" /> appended to the end of <paramref name="path" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.
		/// </exception>
		string ChangeExtension(string path, string extension);

		/// <summary>
		/// Combines two strings into a path.
		/// </summary>
		/// <param name="path1">
		/// The first path to combine.
		/// </param>
		/// <param name="path2">
		/// The second path to combine.
		/// </param>
		/// <returns>
		/// The combined paths. If one of the specified paths is a zero-length string, this method returns the other path. If <paramref name="path2" /> contains an absolute path, this method returns <paramref name="path2" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path1" /> or <paramref name="path2" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path1" /> or <paramref name="path2" /> is <see langword="null" />.
		/// </exception>
		string Combine(string path1, string path2);

		/// <summary>
		/// Returns the directory information for the specified path string.
		/// </summary>
		/// <param name="path">
		/// The path of a file or directory.
		/// </param>
		/// <returns>
		/// Directory information for <paramref name="path" />, or <see langword="null" /> if <paramref name="path" /> denotes a root directory or is null. Returns <see cref="F:System.String.Empty" /> if <paramref name="path" /> does not contain directory information.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// The <paramref name="path" /> parameter contains invalid characters, is empty, or contains only white spaces.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.IO.IOException" />, instead.The <paramref name="path" /> parameter is longer than the system-defined maximum length.
		/// </exception>
		string GetDirectoryName(string path);

		/// <summary>
		/// Returns the extension of the specified path string.
		/// </summary>
		/// <param name="path">
		/// The path string from which to get the extension.
		/// </param>
		/// <returns>
		/// A String containing the extension of the specified path (including the "."), null , or Empty. If path is null , GetExtension returns null. If path does not have extension information, GetExtension returns Empty.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.
		/// </exception>
		string GetExtension(string path);

		/// <summary>
		/// Returns the file name and extension of the specified path string.
		/// </summary>
		/// <param name="path">
		/// The path string from which to obtain the file name and extension.
		/// </param>
		/// <returns>
		/// The characters after the last directory character in <paramref name="path" />. If the last character of <paramref name="path" /> is a directory or volume separator character, this method returns <see cref="F:System.String.Empty" />. If <paramref name="path" /> is <see langword="null" />, this method returns <see langword="null" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.
		/// </exception>
		string GetFileName(string path);

		/// <summary>
		/// Gets a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
		/// </summary>
		/// <returns>
		/// The full path of the temporary file.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs, such as no unique temporary file name is available.- or -This method was unable to create a temporary file.
		/// </exception>
		/// <remarks>
		/// REL-135101: avoid failures when Temp is full.
		/// </remarks>
		string GetTempFileName();

		/// <summary>
		/// Gets a uniquely named with a user-defined suffix, zero-byte temporary file on disk and returns the full path of that file.
		/// </summary>
		/// <param name="fileNameSuffix">
		/// The suffix applied to the unique file name.
		/// </param>
		/// <returns>
		/// The full path of the temporary file.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs, such as no unique temporary file name is available.- or -This method was unable to create a temporary file.
		/// </exception>
		/// <remarks>
		/// REL-135101: avoid failures when Temp is full.
		/// </remarks>
		string GetTempFileName(string fileNameSuffix);

		/// <summary>
		/// Returns the path of the current user's temporary folder.
		/// </summary>
		/// <returns>
		/// The path to the temporary folder, ending with a backslash.
		/// </returns>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permissions.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1024:UsePropertiesWhereAppropriate",
			Justification = "The method name must match the original.")]
		string GetTempPath();

		/// <summary>
		/// Gets a value indicating whether the specified path string contains a root.
		/// </summary>
		/// <param name="path">
		/// The path to test.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="path" /> contains a root; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.
		/// </exception>
		bool IsPathRooted(string path);

		/// <summary>
		/// Determines whether the supplied path matches a standard Windows UNC path.
		/// </summary>
		/// <param name="path">
		/// The path to test.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if a UNC path; otherwise, <see langword="false" />. 
		/// </returns>
		bool IsPathUnc(string path);

		/// <summary>
		/// Normalizes the path by adding the long path prefix when <see cref="SupportLongPaths"/> is <see langword="true" />.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// The normalized path.
		/// </returns>
		string NormalizePath(string path);

		/// <summary>
		/// Gets a value indicating whether <paramref name="path"/> ends with a trailing backslash character.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the path ends with a trailing backslash character; otherwise, <see langword="false" />.
		/// </returns>
		bool PathEndsWithTrailingBackSlash(string path);

		/// <summary>
		/// Trims the leading slash from the supplied path.
		/// </summary>
		/// <param name="path">
		/// The path to trim.
		/// </param>
		/// <returns>
		/// The trimmed path.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path"/> is <see langword="null"/> or empty.
		/// </exception>
		string TrimLeadingSlash(string path);

		/// <summary>
		/// Trims the trailing slash from the supplied path.
		/// </summary>
		/// <param name="path">
		/// The path to trim.
		/// </param>
		/// <returns>
		/// The trimmed path.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path"/> is <see langword="null"/> or empty.
		/// </exception>
		string TrimTrailingSlash(string path);
	}
}
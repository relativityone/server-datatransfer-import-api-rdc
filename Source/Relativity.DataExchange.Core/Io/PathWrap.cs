﻿// ----------------------------------------------------------------------------
// <copyright file="PathWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Represents a class object wrapper for the <see cref="T:System.IO.Path"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class PathWrap : IPath
	{
		/// <summary>
		/// The maximum supported path length before prefixing is required.
		/// </summary>
		public const int MaxSupportedPathLength = WindowsMaxPathLength - WindowsAppendFileNameLength;

		/// <summary>
		/// The UNC path signature.
		/// </summary>
		public static readonly string UncSignature = string.Empty.PadLeft(2, System.IO.Path.DirectorySeparatorChar);

		/// <summary>
		/// The Windows append file name length.
		/// </summary>
		private const int WindowsAppendFileNameLength = 12;

		/// <summary>
		/// The maximum supported windows path length.
		/// </summary>
		private const int WindowsMaxPathLength = 260;

		/// <summary>
		/// The primary directory character that's represented as a string.
		/// </summary>
		private static readonly string DirectorySeparatorString = System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);

		/// <summary>
		/// The alternate directory character that's represented as a string.
		/// </summary>
		private static readonly string AltDirectorySeparatorString = System.IO.Path.AltDirectorySeparatorChar.ToString();

		/// <summary>
		/// The backing field for the custom temporary directory.
		/// </summary>
		private string customTempDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="PathWrap" /> class.
		/// </summary>
		internal PathWrap()
		{
			this.SupportLongPaths = false;
			this.CustomTempPath = string.Empty;
		}

		/// <inheritdoc />
		public bool SupportLongPaths
		{
			get;
			set;
		}

		/// <inheritdoc />
		public string CustomTempPath
		{
			get
			{
				return this.customTempDirectory;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = TryGetFullPath(value);
					if (!string.IsNullOrEmpty(value) && !this.PathEndsWithTrailingBackSlash(value))
					{
						value = this.AddTrailingBackSlash(value);
					}
				}

				this.customTempDirectory = value;
			}
		}

		/// <inheritdoc />
		public string AddTrailingBackSlash(string path)
		{
			if (!this.PathEndsWithTrailingBackSlash(path))
			{
				path = path + DirectorySeparatorString;
			}

			return path;
		}

		/// <inheritdoc />
		public string ChangeExtension(string path, string extension)
		{
			path = this.NormalizePath(path);
			return System.IO.Path.ChangeExtension(path, extension);
		}

		/// <inheritdoc />
		public string Combine(string path1, string path2)
		{
			path1 = this.NormalizePath(path1);
			path2 = this.NormalizePath(path2);
			string path = System.IO.Path.Combine(path1, path2);
			string normalizedPath = path.Replace('/', System.IO.Path.DirectorySeparatorChar)
				.Replace('\\', System.IO.Path.DirectorySeparatorChar);
			return normalizedPath;
		}

		/// <inheritdoc />
		public string ConvertIllegalCharactersInFilename(string fileName)
		{
			const string DefaultReplacement = "_";
			return this.ConvertIllegalCharactersInFilename(fileName, DefaultReplacement);
		}

		/// <inheritdoc />
		public string ConvertIllegalCharactersInFilename(string fileName, string replacement = "_")
		{
			return string.Copy(fileName).Replace("\\", replacement).Replace("/", replacement).Replace("?", replacement)
				.Replace(":", replacement).Replace("*", replacement).Replace(">", replacement).Replace("<", replacement)
				.Replace("|", replacement).Replace("\"", replacement);
		}

		/// <inheritdoc />
		public string GetDirectoryName(string path)
		{
			path = this.NormalizePath(path);
			return System.IO.Path.GetDirectoryName(path);
		}

		/// <inheritdoc />
		public string GetExtension(string path)
		{
			path = this.NormalizePath(path);
			return System.IO.Path.GetExtension(path);
		}

		/// <inheritdoc />
		public string GetFileName(string path)
		{
			path = this.NormalizePath(path);
			return System.IO.Path.GetFileName(path);
		}

		/// <inheritdoc />
		public string GetFullPath(string path)
		{
			path = this.NormalizePath(path);
			string fullPath = TryGetFullPath(path);
			return string.IsNullOrEmpty(fullPath) ? path : fullPath;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1062:Validate arguments of public methods",
			Justification = "The original implementation was explicitly designed to throw NullReferenceException.")]
		public string GetFullyQualifiedPath(Uri baseUri, string path)
		{
			if (this.IsPathFullyQualified(path))
			{
				if (!path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					path += "/";
				}

				return path;
			}

			if (!path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				path = "/" + path;
			}

			if (!path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				path += "/";
			}

			return baseUri.Scheme + Uri.SchemeDelimiter + baseUri.Host + path;
		}

		/// <inheritdoc />
		public string GetTempPath()
		{
			string value = this.CustomTempPath;
			if (string.IsNullOrEmpty(value))
			{
				value = System.IO.Path.GetTempPath();
			}

			return value;
		}

		/// <inheritdoc />
		public string GetTempFileName()
		{
			return this.GetTempFileName(null);
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Maintaining backwards compatibility.")]
		public string GetTempFileName(string fileNameSuffix)
		{
			const string FileNameSeparator = "-";
			if (string.IsNullOrEmpty(fileNameSuffix))
			{
				fileNameSuffix = "rel-default";
			}

			string tempDirectory = this.GetTempPath();
			string fileName = string.Join(
				FileNameSeparator,
				DateTime.Now.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture),
				Guid.NewGuid().ToString("D").ToUpperInvariant(),
				fileNameSuffix);
			string file = this.Combine(tempDirectory, System.IO.Path.ChangeExtension(fileName, "tmp"));

			try
			{
				using (System.IO.File.Create(file))
				{
				}
			}
			catch
			{
				using (System.IO.File.Create(file))
				{
				}
			}

			return file;
		}

		/// <inheritdoc />
		public bool IsPathFullyQualified(string path)
		{
			return new Regex("\\w+:\\/\\/", RegexOptions.IgnoreCase).Matches(path).Count > 0;
		}

		/// <inheritdoc />
		public bool IsPathRooted(string path)
		{
			path = this.NormalizePath(path);
			return System.IO.Path.IsPathRooted(path);
		}

		/// <inheritdoc />
		public bool IsPathUnc(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			return !string.IsNullOrEmpty(path) && path.StartsWith(UncSignature, StringComparison.OrdinalIgnoreCase);
		}

		/// <inheritdoc />
		public string NormalizePath(string path)
		{
			// Note: long and relative paths should never be modified.
			if (!this.SupportLongPaths ||
			    string.IsNullOrEmpty(path) ||
			    path.StartsWith(@"\\?\", StringComparison.OrdinalIgnoreCase) ||
			    !System.IO.Path.IsPathRooted(path))
			{
				return path;
			}

			if (path.Length < MaxSupportedPathLength)
			{
				return path;
			}

			// http://msdn.microsoft.com/en-us/library/aa365247.aspx
			if (this.IsPathUnc(path))
			{
				return @"\\?\UNC\" + path.Substring(2);
			}

			return @"\\?\" + path;
		}

		/// <inheritdoc />
		public bool PathEndsWithTrailingBackSlash(string path)
		{
			var result = !string.IsNullOrEmpty(path)
			             && (path.EndsWith(DirectorySeparatorString, StringComparison.OrdinalIgnoreCase) ||
			                 path.EndsWith(
				                 AltDirectorySeparatorString,
				                 StringComparison.OrdinalIgnoreCase));
			return result;
		}

		/// <inheritdoc />
		public string TrimLeadingSlash(string path)
		{
			return string.IsNullOrEmpty(path)
				? string.Empty
				: path.TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
		}

		/// <inheritdoc />
		public string TrimTrailingSlash(string path)
		{
			return string.IsNullOrEmpty(path)
				? string.Empty
				: path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
		}

		/// <summary>
		/// Tries to retrieve the supplied path.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// The absolute path.
		/// </returns>
		private static string TryGetFullPath(string path)
		{
			// Only catch exceptions that can throw from this method.
			try
			{
				path = System.IO.Path.GetFullPath(path);
				return path;
			}
			catch (ArgumentException)
			{
				return string.Empty;
			}
			catch (System.IO.IOException)
			{
				return string.Empty;
			}
			catch (System.Security.SecurityException)
			{
				return string.Empty;
			}
			catch (System.NotSupportedException)
			{
				return string.Empty;
			}
		}
	}
}
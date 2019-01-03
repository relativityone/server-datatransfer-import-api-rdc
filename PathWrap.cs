// ----------------------------------------------------------------------------
// <copyright file="PathWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.Globalization;

	/// <summary>
	/// Represents a class object wrapper for <see cref="T:System.IO.Path"/> class.
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
		/// The Windows append file name length
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
		/// Initializes a new instance of the <see cref="PathWrap" /> class.
		/// </summary>
		internal PathWrap()
		{
			this.SupportLongPaths = false;
		}

		/// <inheritdoc />
		public bool SupportLongPaths
		{
			get;
			set;
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
		public string GetTempPath()
		{
			return System.IO.Path.GetTempPath();
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
	}
}
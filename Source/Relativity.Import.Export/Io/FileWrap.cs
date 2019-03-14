// ----------------------------------------------------------------------------
// <copyright file="FileWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.IO;

	/// <summary>
	/// Represents a class object wrapper for the <see cref="T:System.IO.File"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class FileWrap : IFile
	{
		/// <summary>
		/// The path instance backing.
		/// </summary>
		private readonly IPath instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileWrap" /> class.
		/// </summary>
		/// <param name="path">
		/// The full path to the file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path"/> is <see langword="null"/>.
		/// </exception>
		internal FileWrap(IPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			this.instance = path;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public void Copy(string sourceFileName, string destFileName)
		{
			sourceFileName = this.instance.NormalizePath(sourceFileName);
			destFileName = this.instance.NormalizePath(destFileName);

			try
			{
				System.IO.File.Copy(sourceFileName, destFileName);
			}
			catch (Exception)
			{
				System.IO.File.Copy(sourceFileName, destFileName);
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			sourceFileName = this.instance.NormalizePath(sourceFileName);
			destFileName = this.instance.NormalizePath(destFileName);

			try
			{
				System.IO.File.Copy(sourceFileName, destFileName, overwrite);
			}
			catch (Exception)
			{
				System.IO.File.Copy(sourceFileName, destFileName, overwrite);
			}
		}

		/// <inheritdoc />
		public int CountLinesInFile(string path)
		{
			using (System.IO.StreamReader streamReader = new System.IO.StreamReader(path))
			{
				int num = 0;
				while (streamReader.ReadLine() != null)
				{
					++num;
				}

				return num;
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public System.IO.FileStream Create(string path)
		{
			path = this.instance.NormalizePath(path);

			try
			{
				return System.IO.File.Create(path);
			}
			catch (Exception)
			{
				return System.IO.File.Create(path);
			}
		}

		/// <inheritdoc />
		public System.IO.FileStream Create(string path, bool append)
		{
			System.IO.FileMode mode = append ? System.IO.FileMode.Append : System.IO.FileMode.Create;
			System.IO.FileAccess access = append ? System.IO.FileAccess.Write : System.IO.FileAccess.ReadWrite;
			return this.Create(path, mode, access, FileShare.None);
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public System.IO.FileStream Create(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share)
		{
			path = this.instance.NormalizePath(path);

			try
			{
				return new System.IO.FileStream(path, mode, access, share);
			}
			catch (Exception)
			{
				return new System.IO.FileStream(path, mode, access, share);
			}
		}

		/// <inheritdoc />
		public System.IO.StreamWriter CreateText(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.File.CreateText(path);
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public void Delete(string path)
		{
			path = this.instance.NormalizePath(path);

			try
			{
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
			}
			catch (Exception)
			{
				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
			}
		}

		/// <inheritdoc />
		public bool Exists(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.File.Exists(path);
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public long GetFileSize(string fileName)
		{
			fileName = this.instance.NormalizePath(fileName);

			try
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
				return fi.Length;
			}
			catch (Exception)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
				return fi.Length;
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public void Move(string sourceFileName, string destFileName)
		{
			try
			{
				System.IO.File.Move(sourceFileName, destFileName);
			}
			catch (Exception)
			{
				System.IO.File.Move(sourceFileName, destFileName);
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "This is OK.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "The try/catch retry implementation is for backwards compatibility only.")]
		public System.IO.FileStream ReopenAndTruncate(string fileName, long length)
		{
			try
			{
				System.IO.FileStream fileStream = new System.IO.FileStream(
					fileName,
					System.IO.FileMode.OpenOrCreate,
					System.IO.FileAccess.ReadWrite,
					System.IO.FileShare.None);
				fileStream.SetLength(length);
				return fileStream;
			}
			catch (Exception)
			{
				System.IO.FileStream fileStream = new System.IO.FileStream(
					fileName,
					System.IO.FileMode.OpenOrCreate,
					System.IO.FileAccess.ReadWrite,
					System.IO.FileShare.None);
				fileStream.SetLength(length);
				return fileStream;
			}
		}
	}
}
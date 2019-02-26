// ----------------------------------------------------------------------------
// <copyright file="StreamWriterWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System.IO;
	using System.Text;

	/// <summary>
	/// Represents an abstract wrapper for the <see cref="T:System.IO.File"/> class.
	/// </summary>
	public class StreamWriterWrap : TextWriter, kCura.WinEDDS.TApi.IStreamWriter
	{
		/// <summary>
		/// The wrapped stream writer instance.
		/// </summary>
		private readonly StreamWriter _instance;

		/// <summary>
		/// Initializes a new instance of the StreamWriter class for the specified file on the specified path, using the specified encoding and default buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
		/// </summary>
		/// <param name="path">
		/// The complete file path to write to.
		/// </param>
		/// <param name="append">
		/// <see langword="true" /> to append data to the file; <see langword="false" /> to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.
		/// </param>
		/// <param name="encoding">
		/// The character encoding to use.
		/// </param>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// Access is denied.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is empty. -or-
		/// <paramref name="path" /> contains the name of a system device (com1, com2, and so on).
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 248 characters, and file names must not exceed 260 characters.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		public StreamWriterWrap(string path, bool append, Encoding encoding)
		{
			_instance = new StreamWriter(path, append, encoding);
		}

		/// <inheritdoc />
		public bool AutoFlush
		{
			get
			{
				return _instance.AutoFlush;
			}

			set
			{
				_instance.AutoFlush = value;
			}
		}

		/// <inheritdoc />
		public Stream BaseStream
		{
			get
			{
				return _instance.BaseStream;
			}
		}

		/// <inheritdoc cref="T:TextWriter.Encoding" />
		public override Encoding Encoding
		{
			get
			{
				return _instance.Encoding;
			}
		}

		/// <inheritdoc cref="T:TextWriter.Close" />
		public override void Close()
		{
			_instance.Close();
		}

		/// <inheritdoc cref="T:TextWriter.Flush" />
		public override void Flush()
		{
			_instance.Flush();
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char value)
		{
			_instance.Write(value);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char[] buffer)
		{
			_instance.Write(buffer);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(string value)
		{
			_instance.Write(value);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char[] buffer, int index, int count)
		{
			_instance.Write(buffer, index, count);
		}

		/// <inheritdoc cref="T:TextWriter.WriteLine" />
		public override void WriteLine(string format, params object[] arg)
		{
			_instance.WriteLine(format, arg);
		}
	}
}
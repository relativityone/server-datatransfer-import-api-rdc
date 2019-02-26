// ----------------------------------------------------------------------------
// <copyright file="StreamWriterWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System.IO;
	using System.Text;

	/// <summary>
	/// Represents a <see cref="T:System.IO.StreamWriter"/> class object wrapper.
	/// </summary>
	public class StreamWriterWrap : TextWriter, IStreamWriter
	{
		/// <summary>
		/// The wrapped stream writer instance.
		/// </summary>
		private readonly StreamWriter instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="StreamWriterWrap"/> class for the specified file on the specified path, using the specified encoding and default buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
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
			this.instance = new StreamWriter(path, append, encoding);
		}

		/// <inheritdoc />
		public bool AutoFlush
		{
			get
			{
				return this.instance.AutoFlush;
			}

			set
			{
				this.instance.AutoFlush = value;
			}
		}

		/// <inheritdoc />
		public Stream BaseStream
		{
			get
			{
				return this.instance.BaseStream;
			}
		}

		/// <inheritdoc cref="T:TextWriter.Encoding" />
		public override Encoding Encoding
		{
			get
			{
				return this.instance.Encoding;
			}
		}

		/// <inheritdoc cref="T:TextWriter.Close" />
		public override void Close()
		{
			this.instance.Close();
		}

		/// <inheritdoc cref="T:TextWriter.Flush" />
		public override void Flush()
		{
			this.instance.Flush();
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char[] buffer)
		{
			this.instance.Write(buffer);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(string value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc cref="T:TextWriter.Write" />
		public override void Write(char[] buffer, int index, int count)
		{
			this.instance.Write(buffer, index, count);
		}

		/// <inheritdoc cref="T:TextWriter.WriteLine" />
		public override void WriteLine(string format, params object[] arg)
		{
			this.instance.WriteLine(format, arg);
		}
	}
}
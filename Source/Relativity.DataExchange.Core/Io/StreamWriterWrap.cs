// ----------------------------------------------------------------------------
// <copyright file="StreamWriterWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System.IO;
	using System.Text;

	/// <summary>
	/// Represents a <see cref="T:System.IO.StreamWriter"/> class object wrapper.
	/// </summary>
	internal class StreamWriterWrap : IStreamWriter
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
		public Stream BaseStream
		{
			get
			{
				return this.instance.BaseStream;
			}
		}

		/// <inheritdoc />
		public void Close()
		{
			this.instance.Close();
		}

		/// <inheritdoc />
		public void Flush()
		{
			this.instance.Flush();
		}

		/// <inheritdoc />
		public void Write(char value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(char[] buffer)
		{
			this.instance.Write(buffer);
		}

		/// <inheritdoc />
		public void Write(string value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(char[] buffer, int index, int count)
		{
			this.instance.Write(buffer, index, count);
		}

		/// <inheritdoc />
		public void WriteLine(string format, params object[] arg)
		{
			this.instance.WriteLine(format, arg);
		}

		/// <inheritdoc />
		public void WriteLine(string value)
		{
			this.instance.WriteLine(value);
		}

		/// <summary>
		/// Dispose this instance.
		/// </summary>
		public void Dispose()
		{
			this.instance.Dispose();
		}

		/// <inheritdoc />
		public void Write(bool value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(decimal value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(double value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(int value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(long value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(object value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(float value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(uint value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(ulong value)
		{
			this.instance.Write(value);
		}

		/// <inheritdoc />
		public void Write(string format, object arg0)
		{
			this.instance.Write(format, arg0);
		}

		/// <inheritdoc />
		public void Write(string format, object[] arg)
		{
			this.instance.Write(format, arg);
		}

		/// <inheritdoc />
		public void Write(string format, object arg0, object arg1)
		{
			this.instance.Write(format, arg0, arg1);
		}

		/// <inheritdoc />
		public void Write(string format, object arg0, object arg1, object arg2)
		{
			this.instance.Write(format, arg0, arg1, arg2);
		}
	}
}
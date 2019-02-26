// ----------------------------------------------------------------------------
// <copyright file="IStreamWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.IO;
	using System.Runtime.Remoting;
	using System.Security;
	using System.Text;

	/// <summary>
	/// Represents an abstract wrapper for the <see cref="T:System.IO.StreamWriter"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IStreamWriter
	{
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="T:System.IO.StreamWriter" /> will flush its buffer to the underlying stream after every call to <see cref="M:System.IO.StreamWriter.Write(System.Char)" />.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> to force <see cref="T:System.IO.StreamWriter" /> to flush its buffer; otherwise, <see langword="false" />.
		/// </returns>
		bool AutoFlush
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the underlying stream that interfaces with a backing store.
		/// </summary>
		/// <returns>
		/// The stream this <see langword="StreamWriter" /> is writing to.
		/// </returns>
		Stream BaseStream
		{
			get;
		}

		/// <summary>
		/// Gets the <see cref="T:System.Text.Encoding" /> in which the output is written.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Text.Encoding" /> specified in the constructor for the current instance, or <see cref="T:System.Text.UTF8Encoding" /> if an encoding was not specified.
		/// </returns>
		Encoding Encoding
		{
			get;
		}

		/// <summary>
		/// Gets an object that controls formatting. 
		/// </summary>
		/// <returns>
		/// The <see cref="IFormatProvider"/> instance.
		/// </returns>
		IFormatProvider FormatProvider
		{
			get;
		}

		/// <summary>
		/// Gets or sets the line terminator string used by the current TextWriter.
		/// </summary>
		/// <returns>
		/// The string.
		/// </returns>
		string NewLine
		{
			get;
			set;
		}

		/// <summary>
		/// Closes the current <see langword="StreamWriter" /> object and the underlying stream.
		/// </summary>
		/// <exception cref="T:System.Text.EncoderFallbackException">
		/// The current encoding does not support displaying half of a Unicode surrogate pair.
		/// </exception>
		void Close();

		/// <summary>
		/// Creates an object that contains all the relevant information required to generate a proxy used to communicate with a remote object.
		/// </summary>
		/// <param name="requestedType">
		/// The Type of the object that the new ObjRef will reference.
		/// </param>
		/// <returns>
		/// Information required to generate a proxy.
		/// </returns>
		[SecurityCritical]
		ObjRef CreateObjRef(Type requestedType);

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
		/// </summary>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current writer is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error has occurred.
		/// </exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">
		/// The current encoding does not support displaying half of a Unicode surrogate pair.
		/// </exception>
		void Flush();

		/// <summary>
		/// Writes a character to the stream.
		/// </summary>
		/// <param name="value">
		/// The character to write to the stream.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and current writer is closed.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and the contents of the buffer cannot be written to the underlying fixed size stream because the <see cref="T:System.IO.StreamWriter" /> is at the end the stream.
		/// </exception>
		void Write(char value);

		/// <summary>
		/// Writes a character array to the stream.
		/// </summary>
		/// <param name="buffer">
		/// A character array containing the data to write. If <paramref name="buffer" /> is <see langword="null" />, nothing is written.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and current writer is closed.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and the contents of the buffer cannot be written to the underlying fixed size stream because the <see cref="T:System.IO.StreamWriter" /> is at the end the stream.
		/// </exception>
		void Write(char[] buffer);

		/// <summary>
		/// Writes a string to the stream.
		/// </summary>
		/// <param name="value">
		/// The string to write to the stream. If <paramref name="value" /> is null, nothing is written.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and current writer is closed.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <see cref="P:System.IO.StreamWriter.AutoFlush" /> is true or the <see cref="T:System.IO.StreamWriter" /> buffer is full, and the contents of the buffer cannot be written to the underlying fixed size stream because the <see cref="T:System.IO.StreamWriter" /> is at the end the stream.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(string value);

		/// <summary>
		/// Writes the text representation of a <see langword="Boolean" /> value to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The <see langword="Boolean" /> value to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(bool value);

		/// <summary>
		/// Writes the text representation of a decimal value to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The decimal value to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(decimal value);

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 8-byte floating-point value to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(double value);

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 4-byte signed integer to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(int value);

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 8-byte signed integer to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(long value);

		/// <summary>
		/// Writes the text representation of an object to the text string or stream by calling the <see langword="ToString" /> method on that object.
		/// </summary>
		/// <param name="value">
		/// The object to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(Object value);

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 4-byte floating-point value to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(float value);

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 4-byte unsigned integer to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(uint value);

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text string or stream.
		/// </summary>
		/// <param name="value">
		/// The 8-byte unsigned integer to write.
		/// </param>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(ulong value);

		/// <summary>
		/// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.
		/// </summary>
		/// <param name="format">
		/// A composite format string (see Remarks).
		/// </param>
		/// <param name="arg0">
		/// The object to format and write.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="format" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.FormatException">
		/// <paramref name="format" /> is not a valid composite format string.-or- The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is one).
		/// </exception>
		void Write(string format, Object arg0);

		/// <summary>
		/// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.
		/// </summary>
		/// <param name="format">
		/// A composite format string (see Remarks).
		/// </param>
		/// <param name="arg">
		/// An object array that contains zero or more objects to format and write.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="format" /> or <paramref name="arg" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.FormatException">
		/// <paramref name="format" /> is not a valid composite format string.-or- The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="arg" /> array.
		/// </exception>
		void Write(string format, Object[] arg);

		/// <summary>
		/// Writes a subarray of characters to the text string or stream.
		/// </summary>
		/// <param name="buffer">
		/// The character array to write data from.
		/// </param>
		/// <param name="index">
		/// The character position in the buffer at which to start retrieving data.
		/// </param>
		/// <param name="count">
		/// The number of characters to write.
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		/// The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="buffer" /> parameter is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="index" /> or <paramref name="count" /> is negative.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		void Write(char[] buffer, int index, int count);

		/// <summary>
		/// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.
		/// </summary>
		/// <param name="format">
		/// A composite format string (see Remarks).
		/// </param>
		/// <param name="arg0">
		/// The first object to format and write.
		/// </param>
		/// <param name="arg1">
		/// The second object to format and write.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="format" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.FormatException">
		/// <paramref name="format" /> is not a valid composite format string.-or- The index of a format item is less than 0 (zero) or greater than or equal to the number of objects to be formatted (which, for this method overload, is two).
		/// </exception>
		void Write(string format, Object arg0, Object arg1);

		/// <summary>
		/// Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.
		/// </summary>
		/// <param name="format">
		/// A composite format string (see Remarks).
		/// </param>
		/// <param name="arg0">
		/// The first object to format and write.
		/// </param>
		/// <param name="arg1">
		/// The second object to format and write.
		/// </param>
		/// <param name="arg2">
		/// The third object to format and write.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="format" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.FormatException">
		/// <paramref name="format" /> is not a valid composite format string.-or- The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is three).
		/// </exception>
		void Write(string format, Object arg0, Object arg1, Object arg2);

		/// <summary>
		/// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.
		/// </summary>
		/// <param name="format">
		/// A composite format string (see Remarks).
		/// </param>
		/// <param name="arg">
		/// An object array that contains zero or more objects to format and write.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// A string or object is passed in as <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The <see cref="T:System.IO.TextWriter" /> is closed.
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		/// </exception>
		/// <exception cref="T:System.FormatException">
		/// <paramref name="format" /> is not a valid composite format string.-or- The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="arg" /> array.
		/// </exception>
		void WriteLine(string format, params object[] arg);
	}
}
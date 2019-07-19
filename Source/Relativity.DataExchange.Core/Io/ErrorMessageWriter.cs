// <copyright file="ErrorMessageWriter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Io
{
	using System;
	using System.IO;
	using Microsoft.VisualBasic;

	/// <summary>
	/// Class that is specialized in writing error messages to the error message file.
	/// </summary>
	/// <typeparam name="T">The type to write.</typeparam>
	public sealed class ErrorMessageWriter<T> : IDisposable
		where T : IErrorArguments
	{
		/// <summary>
		/// This lock is per generic argument.
		/// </summary>
		private static readonly object TLock = new object();
		private static Lazy<StreamWriter> streamForThisType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessageWriter{T}"/> class.
		/// </summary>
		/// <param name="filePath">File location for the error messages.</param>
		public ErrorMessageWriter(string filePath)
		{
			lock (TLock)
			{
				if (string.IsNullOrEmpty(filePath))
				{
					filePath =
						TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix);
				}

				this.FilePath = filePath;

				streamForThisType = new Lazy<StreamWriter>(() => new StreamWriter(
					filePath,
					true,
					System.Text.Encoding.Default));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessageWriter{T}"/> class.
		/// </summary>
		public ErrorMessageWriter()
            : this(string.Empty)
		{
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="ErrorMessageWriter{T}"/> class.
		/// </summary>
		~ErrorMessageWriter()
		{
			ReleaseUnmanagedResources();
		}

		/// <summary>
		/// Gets the location this writer is writing to.
		/// </summary>
		public string FilePath { get; }

		/// <inheritdoc/>
		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Writes an error message to the error file.
		/// </summary>
		/// <param name="toWrite">The line to write.</param>
		public void WriteErrorMessage(T toWrite)
		{
			lock (TLock)
			{
				var lineForFile = toWrite.ValuesForErrorFile().ToCsv(CSVFormat);
				streamForThisType.Value.WriteLine(lineForFile);
			}
		}

		/// <summary>
		/// CSVFormat will take in a string, replace a double quote characters with a pair of double quote characters, then surround the string with double quote characters
		/// This preps it for being written as a field in a CSV file.
		/// </summary>
		/// <param name="fieldValue">The string to convert to CSV format.</param>
		/// <returns>
		/// The converted data.
		/// </returns>
		private static string CSVFormat(string fieldValue)
		{
			var quote = ControlChars.Quote.ToString();
			var doubleQuote = quote + quote;
			var escapedField = fieldValue.Replace(quote, doubleQuote);
			return $"{quote}{escapedField}{quote}";
		}

		private static void ReleaseUnmanagedResources()
		{
			lock (TLock)
			{
				if (streamForThisType.IsValueCreated)
				{
					streamForThisType.Value?.Dispose();
				}
			}
		}
	}
}

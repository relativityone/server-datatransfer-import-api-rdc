// <copyright file="ErrorMessageWriter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	using System.IO;

	using Microsoft.VisualBasic;

	/// <summary>
	/// Class that is specialized in writing error messages to the error message file.
	/// </summary>
	public class ErrorMessageWriter
	{
		private static readonly object Lock = new object();
		private string _errorMessageFileLocation;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessageWriter"/> class.
		/// </summary>
		/// <param name="errorMessageFileLocation">File location for the error messages.</param>
		public ErrorMessageWriter(string errorMessageFileLocation)
		{
			this._errorMessageFileLocation = errorMessageFileLocation;
		}

		/// <summary>
		/// Writes an error message to the error file.
		/// </summary>
		/// <param name="lineNumber">The line number to write.</param>
		/// <param name="message">The message to write.</param>
		/// <param name="identifier">the identifier for this error.</param>
		/// <param name="type">The type of the error.</param>
		public void WriteErrorMessage(string lineNumber, string message, string identifier, string type)
		{
			lock (Lock)
			{
				if (string.IsNullOrEmpty(this._errorMessageFileLocation))
				{
					this._errorMessageFileLocation = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix);
				}

				using (var errorMessageFileWriter = new StreamWriter(
					this._errorMessageFileLocation,
					true,
					System.Text.Encoding.Default))
				{
					errorMessageFileWriter.WriteLine($"{CSVFormat(lineNumber)},{CSVFormat(message)},{CSVFormat(identifier)},{CSVFormat(type)}");
				}
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
	}
}

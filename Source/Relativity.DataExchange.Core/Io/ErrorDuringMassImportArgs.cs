// <copyright file="ErrorDuringMassImportArgs.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	using System.Collections.Generic;

	/// <summary>
	/// Error record before mass import occurs.
	/// These tend to be issues with the data source itself.
	/// </summary>
	public class ErrorDuringMassImportArgs : IErrorArguments
	{
		private readonly string lineNumber;
		private readonly string message;
		private readonly string identifier;
		private readonly string type;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorDuringMassImportArgs"/> class.
		/// </summary>
		/// <param name="lineNumber">The line number of the error, this is the line number of the load file.</param>
		/// <param name="message">Message describing what is wrong.</param>
		/// <param name="identifier">Unique identifier for this error.</param>
		/// <param name="type">Type of the error.</param>
		public ErrorDuringMassImportArgs(string lineNumber, string message, string identifier, string type)
		{
			this.lineNumber = lineNumber;
			this.message = message;
			this.identifier = identifier;
			this.type = type;
		}

		/// <inheritdoc />
		public IEnumerable<string> ValuesForErrorFile()
		{
			yield return this.lineNumber;
			yield return this.message;
			yield return this.identifier;
			yield return this.type;
		}
	}
}
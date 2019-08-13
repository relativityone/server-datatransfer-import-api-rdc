// <copyright file="ErrorBeforeMassImportArgs.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	using System.Collections.Generic;

	/// <summary>
	/// Error record before mass import occurs.
	/// These tend to be issues with the data source itself.
	/// </summary>
	public class ErrorBeforeMassImportArgs : IErrorArguments
	{
		private readonly int currentLineNumber;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBeforeMassImportArgs"/> class.
		/// </summary>
		/// <param name="currentLineNumber">The line number where things went wrong.</param>
		public ErrorBeforeMassImportArgs(int currentLineNumber)
		{
			this.currentLineNumber = currentLineNumber;
		}

		/// <inheritdoc />
		public IEnumerable<string> ValuesForErrorFile()
		{
			yield return this.currentLineNumber.ToString();
		}
	}
}

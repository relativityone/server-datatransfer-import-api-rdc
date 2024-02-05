// <copyright file="LineToIdentifierMappings.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// LineToIdentifierMappings.
	/// </summary>
	public class LineToIdentifierMappings
	{
		private readonly int numberOfRequiredLines;
		private readonly int maxNumberOfLines;
		private readonly Dictionary<long, string> lineToIdentifierMappings = new Dictionary<long, string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="LineToIdentifierMappings"/> class.
		/// </summary>
		public LineToIdentifierMappings()
			: this(
				numberOfRequiredLines: 3 * AppSettings.Instance.ImportBatchSize,
				maxNumberOfLines: 30 * AppSettings.Instance.ImportBatchSize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LineToIdentifierMappings"/> class.
		/// </summary>
		/// <param name="numberOfRequiredLines">Number of required lines.</param>
		/// <param name="maxNumberOfLines">Max number of lines.</param>
		public LineToIdentifierMappings(int numberOfRequiredLines, int maxNumberOfLines)
		{
			this.numberOfRequiredLines = numberOfRequiredLines;
			this.maxNumberOfLines = maxNumberOfLines;
		}

		/// <summary>
		/// Returns identifier of a record for the given line number.
		/// </summary>
		/// <param name="lineNumber">Line number.</param>
		/// <returns>Record identifier or null if identifier for a given line is not available.</returns>
		public string GetIdentifier(long lineNumber)
		{
			return this.lineToIdentifierMappings.TryGetValue(lineNumber, out var identifier)
					   ? identifier
					   : null;
		}

		/// <summary>
		/// Adds line number to record identifier mapping.
		/// </summary>
		/// <param name="lineNumber">Line number.</param>
		/// <param name="identifier">Record identifier.</param>
		public void AddMapping(long lineNumber, string identifier)
		{
			if (this.lineToIdentifierMappings.Count >= this.maxNumberOfLines)
			{
				this.RemoveOutdatedMappings(currentLineNumber: lineNumber);
			}

			this.lineToIdentifierMappings.Add(lineNumber, identifier);
		}

		private void RemoveOutdatedMappings(long currentLineNumber)
		{
			long minimumLineNumber = currentLineNumber + 1 - this.numberOfRequiredLines;

			var allLineNumbers = this.lineToIdentifierMappings.Keys.ToList();
			foreach (var lineNumber in allLineNumbers)
			{
				if (lineNumber < minimumLineNumber)
				{
					this.lineToIdentifierMappings.Remove(lineNumber);
				}
			}
		}
	}
}

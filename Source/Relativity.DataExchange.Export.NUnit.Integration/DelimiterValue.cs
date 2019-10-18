// ----------------------------------------------------------------------------
// <copyright file="DelimiterValue.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	public class DelimiterValue
	{
		public DelimiterValue(char multiRecordDelimiter, char nestedValueDelimiter, char newlineDelimiter, char quoteDelimiter, char recordDelimiter)
		{
			this.MultiRecordDelimiter = multiRecordDelimiter;
			this.NestedValueDelimiter = nestedValueDelimiter;
			this.NewlineDelimiter = newlineDelimiter;
			this.QuoteDelimiter = quoteDelimiter;
			this.RecordDelimiter = recordDelimiter;
		}

		public char MultiRecordDelimiter { get; set; }

		public char NestedValueDelimiter { get; set; }

		public char NewlineDelimiter { get; set; }

		public char QuoteDelimiter { get; set; }

		public char RecordDelimiter { get; set; }

		public override string ToString()
		{
			return $"({this.MultiRecordDelimiter}, {this.NestedValueDelimiter}, {this.NewlineDelimiter}, {this.QuoteDelimiter}, {this.RecordDelimiter})";
		}
	}
}

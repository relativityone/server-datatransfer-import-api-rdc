// ----------------------------------------------------------------------------
// <copyright file="DelimiterDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration.Dto
{
	public class DelimiterDto
	{
		public DelimiterDto(char multiRecordDelimiter, char nestedValueDelimiter, char newlineDelimiter, char quoteDelimiter, char recordDelimiter)
		{
			this.MultiRecordDelimiter = multiRecordDelimiter;
			this.NestedValueDelimiter = nestedValueDelimiter;
			this.NewlineDelimiter = newlineDelimiter;
			this.QuoteDelimiter = quoteDelimiter;
			this.RecordDelimiter = recordDelimiter;
		}

		public char MultiRecordDelimiter { get; }

		public char NestedValueDelimiter { get; }

		public char NewlineDelimiter { get; }

		public char QuoteDelimiter { get; }

		public char RecordDelimiter { get; }

		public override string ToString()
		{
			return $"({this.MultiRecordDelimiter}, {this.NestedValueDelimiter}, {this.NewlineDelimiter}, {this.QuoteDelimiter}, {this.RecordDelimiter})";
		}
	}
}

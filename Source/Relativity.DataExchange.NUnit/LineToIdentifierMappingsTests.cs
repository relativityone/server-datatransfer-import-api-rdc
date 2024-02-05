// <copyright file="LineToIdentifierMappingsTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit
{
	using System.Linq;

	using FluentAssertions;

	using global::NUnit.Framework;

	[TestFixture]
	internal class LineToIdentifierMappingsTests
	{
		private const int NumberOfRequiredLines = 10;
		private const int MaxNumberOfLines = 20;
		private LineToIdentifierMappings sut;

		[SetUp]
		public void SetUp()
		{
			this.sut = new LineToIdentifierMappings(NumberOfRequiredLines, MaxNumberOfLines);
		}

		[Test]
		public void GetIdentifier_ReturnsNull_WhenIdentifierWasNotAdded()
		{
			// act
			var actualIdentifier = this.sut.GetIdentifier(5);

			// assert
			actualIdentifier.Should().BeNull();
		}

		[Test]
		public void GetIdentifier_ReturnsValue_WhenItWasAddedToMapping()
		{
			// arrange
			const string identifier = "MyID111";
			this.sut.AddMapping(5, identifier);

			// act
			var actualIdentifier = this.sut.GetIdentifier(5);

			// assert
			actualIdentifier.Should().Be(identifier);
		}

		[Test]
		public void GetIdentifier_ReturnsAllValues_WhenMaxNumberNotExceeded()
		{
			// arrange
			var linesToAdd = Enumerable.Range(0, MaxNumberOfLines).ToArray();
			foreach (var line in linesToAdd)
			{
				this.sut.AddMapping(line, line.ToString());
			}

			foreach (var line in linesToAdd)
			{
				// act
				var actualIdentifier = this.sut.GetIdentifier(line);

				// assert
				string expectedIdentifier = line.ToString();
				actualIdentifier.Should().Be(expectedIdentifier);
			}
		}

		[Test]
		public void AddMapping_RemovesOutdatedLines_WhenMaxNumberExceeded()
		{
			// arrange
			var linesToAdd = Enumerable.Range(0, MaxNumberOfLines).ToArray();
			foreach (var line in linesToAdd)
			{
				this.sut.AddMapping(line, line.ToString());
			}

			// act
			this.sut.AddMapping(MaxNumberOfLines, MaxNumberOfLines.ToString());

			// assert
			var minimumLineNumber = MaxNumberOfLines + 1 - NumberOfRequiredLines;
			var removedLines = Enumerable.Range(0, minimumLineNumber);
			var remainingLines = Enumerable.Range(minimumLineNumber, NumberOfRequiredLines);

			foreach (var line in removedLines)
			{
				var actualIdentifier = this.sut.GetIdentifier(line);
				actualIdentifier.Should().BeNull();
			}

			foreach (var line in remainingLines)
			{
				var expectedIdentifier = line.ToString();
				var actualIdentifier = this.sut.GetIdentifier(line);
				actualIdentifier.Should().Be(expectedIdentifier);
			}
		}
	}
}

// <copyright file="HashingHelperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.NUnit.Helpers
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Helpers;

	public class HashingHelperTests
	{
		[TestCaseSource(typeof(HashingHelperTestCases), nameof(HashingHelperTestCases.ShouldCalculateSHA256HashTestCaseData))]
		public void ShouldCalculateSHA256Hash(string value, string expected)
		{
			// ACT
			var result = HashingHelper.CalculateSHA256Hash(value);

			// ASSERT
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ShouldCalculateSHA256HashForNullThrowException()
		{
			// ACT && ASSERT
			ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => HashingHelper.CalculateSHA256Hash(null));

			// Assert
			Console.WriteLine(exception.Message);
		}
	}
}

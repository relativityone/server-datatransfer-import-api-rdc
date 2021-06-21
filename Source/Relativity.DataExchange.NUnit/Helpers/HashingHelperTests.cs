// <copyright file="HashingHelperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.NUnit.Helpers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
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
		}

		[Test]
		public void ShouldCalculateSHA256HashParallel()
		{
			// ARRANGE
			int threadsCount = 10;
			int hashesCount = 10000;
			string value = "C:/photo.jpg";
			string excpected = "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539";

			// ACT && ASSERT
			var tasks = Enumerable
				.Range(1, threadsCount)
				.Select(i => Task.Run(
					() =>
						{
							for (int j = 0; j < hashesCount; j++)
							{
								var result = HashingHelper.CalculateSHA256Hash(value);
								Assert.AreEqual(excpected, result);
							}
						}))
				.ToArray();

			Task.WaitAll(tasks);
		}
	}
}
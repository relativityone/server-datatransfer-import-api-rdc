// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatingLoggerDecoratorTestCases.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Creates TestCaseData for AggregatingLoggerDecorator unit tests"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Logger;

	public static class AggregatingLoggerDecoratorTestCases
	{
		public static IEnumerable<TestCaseData> ShouldLogToBothLoggersTestCaseData
		{
			get
			{
				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg" },
					true);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg", 1050 },
					true);

				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg", new[] { 0 } },
					true);

				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg".Secure() },
					true);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg", 1050, new[] { 0, 1 } },
					true);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg".Secure(), 1050.Secure() },
					true);

				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg" },
					false);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg", 1050 },
					false);

				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg", new[] { 0 } },
					false);

				yield return new TestCaseData(
					"File {path} closed",
					new object[] { "C:/photo.jpg".Secure() },
					false);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg", 1050, new[] { 0, 1 } },
					false);

				yield return new TestCaseData(
					"File {path} closed. File size {size} kB.",
					new object[] { "C:/photo.jpg".Secure(), 1050.Secure() },
					false);
			}
		}
	}
}

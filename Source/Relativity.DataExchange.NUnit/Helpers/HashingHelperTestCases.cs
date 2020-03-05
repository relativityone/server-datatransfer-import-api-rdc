// <copyright file="HashingHelperTestCases.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.NUnit.Helpers
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	public static class HashingHelperTestCases
	{
		public static IEnumerable<TestCaseData> ShouldCalculateSHA256HashTestCaseData
		{
			get
			{
				{
					yield return new TestCaseData(
						"C:/photo.jpg",
						"7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539");

					yield return new TestCaseData(
						1000.ToString(),
						"40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58");

					yield return new TestCaseData(
						(new int[] { 1, 2, 3 }).ToString(),
						"3d2652dd050837302d168a719c23eef8db0057c12981bac52677167754df7a96");

					yield return new TestCaseData(
						string.Empty,
						"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
				}
			}
		}
	}
}

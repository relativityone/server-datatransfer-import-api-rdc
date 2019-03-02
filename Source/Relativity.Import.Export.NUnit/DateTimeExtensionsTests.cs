// -----------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CollectionExtensions"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="DateTimeExtensions"/> tests.
	/// </summary>
	[TestFixture]
    public static class DateTimeExtensionsTests
	{
		/// <summary>
		/// Gets the test case data source.
		/// </summary>
		/// <value>
		/// The <see cref="IEnumerable"/> instance.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used via NUnit's TestCaseSource feature.")]
		public static IEnumerable DateTimeTestCaseSource
		{
			get
			{
				yield return new TestCaseData(new System.DateTime(2012, 10, 31), "20121031 00:00:00.000");
				yield return new TestCaseData(new System.DateTime(2012, 10, 31, 15, 29, 40), "20121031 15:29:40.000");
				yield return new TestCaseData(new System.DateTime(2012, 1, 5, 3, 9, 4), "20120105 03:09:04.000");
				yield return new TestCaseData(new System.DateTime(2012, 10, 31, 15, 29, 40, 655), "20121031 15:29:40.655");
				yield return new TestCaseData(new System.DateTime(600, 10, 31, 15, 29, 40, 655), "06001031 15:29:40.655");
				yield return new TestCaseData(new System.DateTime(2012, 1, 5, 3, 9, 4, 8), "20120105 03:09:04.008");
			}
		}

		[Test]
		[TestCaseSource(nameof(DateTimeTestCaseSource))]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldConvertToSqlCultureNeutralString(DateTime input, string expected)
        {
	        string value = input.ToSqlCultureNeutralString();
			Assert.That(value, Is.EqualTo(expected));
		}
	}
}
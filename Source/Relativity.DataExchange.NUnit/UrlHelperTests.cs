// -----------------------------------------------------------------------------------------------------
// <copyright file="UrlHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents the <see cref="UrlHelper"/> tests.
	/// </summary>
	public class UrlHelperTests
	{
		/// <summary>
		/// Gets the test case source used to validate combining absolute and relative URL components.
		/// </summary>
		/// <value>
		/// The <see cref="IEnumerable"/> instance.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used via NUnit's TestCaseSource feature.")]
		public static IEnumerable BaseUrlAndCombineTestCaseSource
		{
			get
			{
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local", "Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local", "/Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local", "/Relativity.Distributed/", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local/", "Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local/", "/Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local/", "/Relativity.Distributed/", "https://VM-T005WEB001.T005.relativityone.local/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443", "Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443", "/Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443", "/Relativity.Distributed/", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443/", "Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443/", "/Relativity.Distributed", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://VM-T005WEB001.T005.relativityone.local:8443/", "/Relativity.Distributed/", "https://VM-T005WEB001.T005.relativityone.local:8443/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI/", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI/", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com/RelativityWebAPI/", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI/", "Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI/", "/Relativity.Distributed", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
				yield return new TestCaseData("https://ctus014128-t005.r1.kcura.com:443/RelativityWebAPI/", "/Relativity.Distributed/", "https://ctus014128-t005.r1.kcura.com/Relativity.Distributed/");
			}
		}

		/// <summary>
		/// Gets the test case source used to validate non well-formed URL's.
		/// </summary>
		/// <value>
		/// The <see cref="IEnumerable"/> instance.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used via NUnit's TestCaseSource feature.")]
		public static IEnumerable NotWellFormedUrlTestCaseSource
		{
			get
			{
				yield return new TestCaseData(@"ctus014128-t005.r1.kcura.com");
				yield return new TestCaseData(@"http//ctus014128-t005.r1.kcura.com");
				yield return new TestCaseData(@"http:\\\ctus014128-t005.r1.kcura.com");
			}
		}

		[Test]
		[TestCaseSource(nameof(BaseUrlAndCombineTestCaseSource))]
		[Category(TestCategories.Framework)]
		public void ShouldGetTheBaseUrlAndCombineTheParts(string absoluteUrl, string relativeUrl, string expected)
		{
			// ACT
			string returnedPath = UrlHelper.Combine(UrlHelper.GetBaseUrl(absoluteUrl), relativeUrl);

			// ASSERT
			Assert.That(returnedPath, Is.EqualTo(expected).IgnoreCase);
		}

		[Test]
		[TestCaseSource(nameof(NotWellFormedUrlTestCaseSource))]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenGettingTheBaseUrlFromTheNotWellFormedUrl(string url)
		{
			// ACT/ASSERT
			Assert.Throws<UriFormatException>(() => UrlHelper.GetBaseUrl(url));
		}

		[Test]
		[TestCaseSource(nameof(NotWellFormedUrlTestCaseSource))]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenCombiningTheNotWellFormedUrl(string url)
		{
			// ACT/ASSERT
			Assert.Throws<UriFormatException>(() => UrlHelper.Combine(url, "RelativityWebAPI"));
		}
	}
}
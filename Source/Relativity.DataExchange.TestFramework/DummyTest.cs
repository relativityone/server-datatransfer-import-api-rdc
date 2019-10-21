// ----------------------------------------------------------------------------
// <copyright file="DummyTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using NUnit.Framework;

	/// <summary>
	/// That dummy class is required to avoid "Has no TestFixtures" error when running tests from a console runner.
	/// </summary>
	[TestFixture]
	public static class DummyTest
	{
		[Test]
		public static void Test()
		{
			Assert.Pass();
		}
	}
}

// -----------------------------------------------------------------------------------------------------
// <copyright file="NullAuthTokenProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange;

	[TestFixture]
	public class NullAuthTokenProviderTests
	{
		[Test]
		public void ItShouldGenerateEmptyToken()
		{
			// ARRANGE
			NullAuthTokenProvider subjectUnderTest = new NullAuthTokenProvider();

			// ACT
			string tokenGenerated = subjectUnderTest.GenerateToken();

			// ASSERT
			Assert.IsEmpty(tokenGenerated);
		}
	}
}
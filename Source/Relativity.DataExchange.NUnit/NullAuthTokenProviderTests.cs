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
			// arrange
			NullAuthTokenProvider subjectUnderTest = new NullAuthTokenProvider();

			// act
			string tokenGenerated = subjectUnderTest.GenerateToken();

			// assert
			Assert.IsEmpty(tokenGenerated);
		}
	}
}
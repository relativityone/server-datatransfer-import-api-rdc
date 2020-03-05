// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurePropertyValueTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="HashingLoggerDecorator"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
    using global::NUnit.Framework;
    using global::NUnit.Framework.Internal;

    using Relativity.DataExchange.Logger;

    [TestFixture]
	public class SecurePropertyValueTests
	{
		private static object[] testCaseData = { new object(), "value", 1, new[] { 1, 2, 3 }, 'a', null };

		[Test]
		[TestCaseSource(nameof(testCaseData))]
		public void ShouldToStringMethodReturnsOriginalValue(object value)
		{
			// ARRANGE
			string expected = value?.ToString() ?? string.Empty;
			SecurePropertyValueBase securePropertyValue = value.Secure();

			// ACT
			string result = securePropertyValue.ToString();

			// ARRANGE
			Assert.AreEqual(expected, result);
		}
	}
}

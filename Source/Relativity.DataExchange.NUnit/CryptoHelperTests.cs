// -----------------------------------------------------------------------------------------------------
// <copyright file="CryptoHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents <see cref="CollectionExtensions"/> tests.
	/// </summary>
	[TestFixture]
	public static class CryptoHelperTests
	{
		[Test]
		[TestCase("smoketestuser@relativity.com")]
		[TestCase("Test1234!")]
		[TestCase("serviceaccount@relativity.com")]
		[TestCase("sa")]
		[TestCase("P@ssw0rd@1")]
		[Category(TestCategories.TestFramework)]
		public static void ShouldEncryptAndDecryptTheValue(string input)
		{
			string encrypted = CryptoHelper.Encrypt(input);
			string decrypted = CryptoHelper.Decrypt(encrypted);
			Console.WriteLine($"Plain-text: {input}, Encrypted: {encrypted}");
			Assert.That(decrypted, Is.EqualTo(input));
		}
	}
}
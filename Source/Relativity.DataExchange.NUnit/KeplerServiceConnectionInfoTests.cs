// -----------------------------------------------------------------------------------------------------
// <copyright file="KeplerServiceConnectionInfoTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="KeplerServiceConnectionInfo"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Net;
	using global::NUnit.Framework;
	using Relativity.DataExchange.Service;

	[TestFixture]
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "this should throw, and the assignment does not matter.")]
	public class KeplerServiceConnectionInfoTests : SerializationTestsBase
	{
		[Test]
		public void KeplerServiceConnectionInfoShouldThrowOnInvalidConstructorArguments()
		{
			Assert.Throws<ArgumentException>(
				() => new KeplerServiceConnectionInfo(
					new Uri("../Resources/Styles/Shared.xaml", UriKind.Relative),
					new NetworkCredential("tim", "pazzword")));

			Assert.Throws<ArgumentNullException>(
				() => new KeplerServiceConnectionInfo(
					null,
					new NetworkCredential("tim", "pazzword")));

			Assert.Throws<ArgumentNullException>(
				() => new KeplerServiceConnectionInfo(
					new Uri("https://example.com/Resources/Styles/Shared.xaml"),
					null));

			Assert.Throws<ArgumentException>(
				() => new KeplerServiceConnectionInfo(
					new Uri(string.Empty, UriKind.Relative),
					new NetworkCredential("tim", "pazzword")));

			Assert.Throws<ArgumentException>(
				() => new KeplerServiceConnectionInfo(
					new Uri("https://example.com/Resources/Styles/Shared.xaml"),
					new NetworkCredential("XxX_BearerTokenCredentials_XxX", (string)null)));

			Assert.Throws<ArgumentException>(
				() => new KeplerServiceConnectionInfo(
					new Uri("https://example.com/Resources/Styles/Shared.xaml"),
					new NetworkCredential("XxX_BearerTokenCredentials_xxX", (string)null)));
		}
	}
}
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
	using Relativity.Services.ServiceProxy;

	[TestFixture]
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "this should throw, and the assignment does not matter.")]
	public class KeplerServiceConnectionInfoTests : SerializationTestsBase
	{
		private readonly Uri validUri = new Uri("https://iapi.relativity.com");

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

		[Test]
		public void ShouldUpdatePasswordInOriginalCredentials()
		{
			// arrange
			const string updatedPassword = "UpdatedPassword";
			var originalCredentials = new NetworkCredential(Constants.OAuthWebApiBearerTokenUserName, "password");

			var sut = new KeplerServiceConnectionInfo(this.validUri, originalCredentials);

			var updatedCredentials = new NetworkCredential(Constants.OAuthWebApiBearerTokenUserName, updatedPassword);

			// act
			sut.UpdateCredentials(updatedCredentials);

			// assert
			Assert.That(originalCredentials.Password, Is.EqualTo(updatedPassword), "Original password should be updated.");
			Assert.That(sut.Credentials, Is.InstanceOf<BearerTokenCredentials>());

			var credentialsAsBearerTokenCredentials = sut.Credentials as BearerTokenCredentials;
			Assert.That(credentialsAsBearerTokenCredentials?.Token, Is.EqualTo(updatedPassword), "Credentials should contain updated password.");
		}

		[Test]
		public void ShouldNotRefreshCredentialsWhenPasswordNotChanged()
		{
			// arrange
			var originalCredentials = new NetworkCredential(Constants.OAuthWebApiBearerTokenUserName, "password");

			var sut = new KeplerServiceConnectionInfo(this.validUri, originalCredentials);

			// act
			bool hasRefreshedCredentials = sut.RefreshCredentials();

			// assert
			Assert.That(hasRefreshedCredentials, Is.False, "Password has not changed.");
		}

		[Test]
		public void ShouldRefreshCredentialsWhenPasswordHasChanged()
		{
			// arrange
			const string updatedPassword = "UpdatedPassword";
			var originalCredentials = new NetworkCredential(Constants.OAuthWebApiBearerTokenUserName, "password");

			var sut = new KeplerServiceConnectionInfo(this.validUri, originalCredentials);

			originalCredentials.Password = updatedPassword;

			// act
			bool hasRefreshedCredentials = sut.RefreshCredentials();

			// assert
			Assert.That(hasRefreshedCredentials, Is.True, "Password has changed.");

			Assert.That(sut.Credentials, Is.InstanceOf<BearerTokenCredentials>());

			var credentialsAsBearerTokenCredentials = sut.Credentials as BearerTokenCredentials;
			Assert.That(credentialsAsBearerTokenCredentials?.Token, Is.EqualTo(updatedPassword), "Credentials should contain updated password.");
		}
	}
}
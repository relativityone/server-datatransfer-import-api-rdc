// -----------------------------------------------------------------------------------------------------
// <copyright file="WebServiceTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit.Integration
{
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents the base class for web-service class objects.
	/// </summary>
	public abstract class WebServiceTestsBase
	{
		/// <summary>
		/// The default web-service timeout in milliseconds.
		/// </summary>
		protected const int DefaultTimeOutMilliseconds = 600000;

		/// <summary>
		/// Gets the cookie container.
		/// </summary>
		/// <value>
		/// The <see cref="CookieContainer"/> value.
		/// </value>
		protected CookieContainer CookieContainer
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the credentials.
		/// </summary>
		/// <value>
		/// The <see cref="ICredentials"/> value.
		/// </value>
		protected ICredentials Credentials
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the Relativity WebAPI URL and ensure the URL includes a trailing slash.
		/// </summary>
		/// <value>
		/// The URL string.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1056:UriPropertiesShouldNotBeStrings",
			Justification = "A string data type is being used due to numerous existing expectations.")]
		protected string RelativityWebApiUrl
		{
			get
			{
				string url = this.TestParameters.RelativityWebApiUrl.ToString();
				if (url.LastIndexOf('/') != url.Length - 1)
				{
					url += '/';
				}

				return url;
			}
		}

		/// <summary>
		/// Gets the integration test parameters.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> value.
		/// </value>
		protected IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.CookieContainer = new CookieContainer();
			this.AssignTestSettings();
			this.Credentials = new NetworkCredential(this.TestParameters.RelativityUserName, this.TestParameters.RelativityPassword);
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");
		}

		/// <summary>
		/// Assign the test parameters. This should always be called from methods with <see cref="SetUpAttribute"/> or <see cref="OneTimeSetUpAttribute"/>.
		/// </summary>
		private void AssignTestSettings()
		{
			if (this.TestParameters == null)
			{
				this.TestParameters = AssemblySetup.TestParameters.DeepCopy();
			}
		}
	}
}
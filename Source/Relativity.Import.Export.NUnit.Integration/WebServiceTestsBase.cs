// -----------------------------------------------------------------------------------------------------
// <copyright file="WebServiceTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit.Integration
{
	using System.Net;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

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
		/// Gets the Relativity instance.
		/// </summary>
		/// <value>
		/// The <see cref="RelativityInstance"/> value.
		/// </value>
		internal RelativityInstanceInfo RelativityInstance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <value>
		/// The <see cref="IAppSettings"/> instance.
		/// </value>
		protected IAppSettings AppSettings
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the cancellation token source.
		/// </summary>
		/// <value>
		/// The <see cref="CancellationTokenSource"/> instance.
		/// </value>
		protected CancellationTokenSource CancellationTokenSource
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the mocked Relativity Logging logger.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> mock instance.
		/// </value>
		protected Mock<Relativity.Logging.ILog> Logger
		{
			get;
			private set;
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
			this.AssignTestSettings();
			this.AppSettings = AppSettingsManager.Create(true);
			this.CancellationTokenSource = new CancellationTokenSource();
			this.Logger = new Mock<Relativity.Logging.ILog>();
			this.RelativityInstance = new RelativityInstanceInfo
				                          {
					                          Credentials = new NetworkCredential(
						                          this.TestParameters.RelativityUserName,
						                          this.TestParameters.RelativityPassword),
					                          Host = this.TestParameters.RelativityUrl,
					                          WebApiServiceUrl = this.TestParameters.RelativityWebApiUrl,
				                          };
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");
			this.OnSetup();
		}

		/// <summary>
		/// Called when the test setup is executed.
		/// </summary>
		protected virtual void OnSetup()
		{
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
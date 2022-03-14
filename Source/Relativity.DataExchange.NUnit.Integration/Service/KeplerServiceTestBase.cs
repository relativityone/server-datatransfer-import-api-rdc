// <copyright file="KeplerServiceTestBase.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Net;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Kepler;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Logging;

	public class KeplerServiceTestBase
	{
		protected const int NonExistingWorkspaceId = 0;
		protected const int NonExistingProductionId = 0;
		protected const int NonExistingArtifactTypeId = 0;
		protected const int NonExistingSearchId = 0;
		protected const int NonExistingArtifactId = 0;
		protected const int NonExistingFolderId = 0;
		protected const int NonExistingFieldId = 0;
		protected const int WorkspaceRootFolderId = 1003697;

		private bool? useKeplerOriginalValue;

		public KeplerServiceTestBase(bool useKepler)
		{
			this.UseKepler = useKepler;
			if (useKepler)
			{
				RelativityVersionChecker.SkipTestIfRelativityVersionIsLowerThan(IntegrationTestHelper.IntegrationTestParameters, RelativityVersion.Sundrop);
			}

			if (!useKepler)
			{
				RelativityVersionChecker.SkipTestIfRelativityVersionIsGreaterOrEqual(IntegrationTestHelper.IntegrationTestParameters, RelativityVersion.WhiteSedge);
			}

			this.Logger = new TestNullLogger();
		}

		protected bool UseKepler { get; }

		protected IntegrationTestParameters TestParameters { get; private set; }

		protected NetworkCredential Credential { get; private set; }

		protected RelativityInstanceInfo RelativityInstanceInfo { get; private set; }

		protected CookieContainer CookieContainer { get; private set; }

		protected Func<string> CorrelationIdFunc { get; private set; }

		protected ILog Logger { get; }

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
																			 | SecurityProtocolType.Tls11
																			 | SecurityProtocolType.Tls12;

			this.TestParameters = AssemblySetup.TestParameters;
			Assume.That(this.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			this.useKeplerOriginalValue = AppSettings.Instance.UseKepler;
			AppSettings.Instance.UseKepler = this.UseKepler;

			// This will reduce the time of tests for WebApi
			AppSettings.Instance.WaitBeforeReconnect = 0;
			AppSettings.Instance.WaitTimeBetweenReLogOn = 0;

			ManagerFactory.InvalidateCache();
			WebApiVsKeplerFactory.InvalidateCache();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			AppSettings.Instance.UseKepler = this.useKeplerOriginalValue;
		}

		[SetUp]
		public void Setup()
		{
			this.Credential = new NetworkCredential(
				this.TestParameters.RelativityUserName,
				this.TestParameters.RelativityPassword);
			this.CookieContainer = new CookieContainer();
			this.RelativityInstanceInfo = new RelativityInstanceInfo
			{
				Credentials = this.Credential,
				CookieContainer = CookieContainer,
				WebApiServiceUrl = new Uri(AppSettings.Instance.WebApiServiceUrl),
			};

			string correlationId = Guid.NewGuid().ToString();
			this.CorrelationIdFunc = () => correlationId;
		}

		protected IResolveConstraint GetExpectedExceptionConstraintForNonExistingWorkspace(int workspaceId)
		{
			string expectedExceptionMessage;
			if (this.UseKepler)
			{
				expectedExceptionMessage =
					"PermissionCheckInterceptor." +
					" InnerExceptionType: Relativity.Core.Exception.InvalidAppArtifactID," +
					$" InnerExceptionMessage: Could not retrieve ApplicationID #{workspaceId}.";
			}
			else
			{
				expectedExceptionMessage = $"Could not retrieve ApplicationID #{workspaceId}.";
			}

			return Throws.Exception.InstanceOf<SoapException>()
				.With.Message.Contains(expectedExceptionMessage);
		}
	}
}

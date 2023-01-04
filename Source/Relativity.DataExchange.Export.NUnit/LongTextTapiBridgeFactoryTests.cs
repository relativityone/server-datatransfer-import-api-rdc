// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextTapiBridgeFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Net;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	[TestFixture]
	public class LongTextTapiBridgeFactoryTests
	{
		private Mock<IAppSettings> _appSettings;
		private TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private TestNullLogger _logger;
		private LongTextTapiBridgeFactory _instance;

		[SetUp]
		public void SetUp()
		{
			this._appSettings = new Mock<IAppSettings>();
			var testLogger = new TestNullLogger();
			this._logger = testLogger;
			this._tapiBridgeParametersFactory = new TapiBridgeParametersFactory(
				new ExportFile(12) { Credential = new NetworkCredential(), CaseInfo = new CaseInfo { ArtifactID = 1 } },
				new ExportConfig(),
				this._appSettings.Object);
			this._instance = new LongTextTapiBridgeFactory(
				this._tapiBridgeParametersFactory,
				this._logger,
				this._appSettings.Object,
				CancellationToken.None);
		}

		[Test]
		public void ItShouldOverrideTheBridgeParameters()
		{
			// ARRANGE
			this._appSettings.SetupGet(x => x.TapiForceAsperaClient).Returns(true);
			this._appSettings.SetupGet(x => x.TapiForceClientCandidates).Returns("Aspera;Http;");
			this._appSettings.SetupGet(x => x.TapiForceFileShareClient).Returns(true);
			this._appSettings.SetupGet(x => x.TapiForceHttpClient).Returns(false);
			this._appSettings.SetupGet(x => x.HttpTimeoutSeconds).Returns(100);
			this._appSettings.SetupGet(x => x.HttpExtractedTextTimeoutSeconds).Returns(200);

			// ACT
			ITapiBridge bridge = this._instance.Create();

			// ASSERT
			Assert.That(bridge.Parameters.ForceAsperaClient, Is.False);
			Assert.That(bridge.Parameters.ForceClientCandidates, Is.Null.Or.Empty);
			Assert.That(bridge.Parameters.ForceFileShareClient, Is.False);
			Assert.That(bridge.Parameters.ForceHttpClient, Is.True);
			Assert.That(bridge.Parameters.TimeoutSeconds, Is.EqualTo(200));
		}
	}
}
// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a base class for <see cref="TapiBridgeBase"/> derived classes.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Net;
	using System.Threading;

    using global::NUnit.Framework;

	using Moq;

    using Relativity.Import.Export.TestFramework;
    using Relativity.Import.Export.Transfer;
    using Relativity.Transfer;

	/// <summary>
	/// Represents a base class for <see cref="TapiBridgeBase"/> derived classes.
	/// </summary>
	/// <typeparam name="TTapiBridge">
	/// The TAPI bridge type under test.
	/// </typeparam>
	public abstract class TapiBridgeTestsBase<TTapiBridge>
		where TTapiBridge : TapiBridgeBase
	{
		/// <summary>
		/// The real object service backing.
		/// </summary>
		private readonly TapiObjectService realObjectService = new TapiObjectService();

		protected Mock<ITapiObjectService> MockTapiObjectService { get; private set; }

		protected Mock<ITransferJob> MockTransferJob { get; private set; }

		protected Mock<ITransferLog> MockTransferLogger { get; private set; }

		protected Mock<IRelativityTransferHost> MockRelativityTransferHost { get; private set; }

		protected Mock<ITransferClient> MockTransferClient { get; private set; }

		protected Mock<IFileSystemService> MockTransferFileSystemService { get; private set; }

		protected TTapiBridge TapiBridgeInstance { get; set; }

		protected ClientConfiguration TestClientConfiguration { get; private set; }

		protected int TestWorkspaceId { get; private set; }

		[SetUp]
		public void Setup()
		{
			this.TestClientConfiguration = new ClientConfiguration();
			this.TestWorkspaceId = RandomHelper.NextInt32(1111111, 9999999);
			this.MockTransferFileSystemService = new Mock<IFileSystemService>();
			this.MockTransferFileSystemService.Setup(x => x.GetFileName(It.IsAny<string>()))
				.Returns(RandomHelper.NextString(10, 20));
			this.MockTransferJob = new Mock<ITransferJob>();
			this.MockTransferClient = new Mock<ITransferClient>();
			this.MockTransferClient.SetupGet(x => x.Client).Returns(this.TestClientConfiguration.Client);
			this.MockTransferClient.SetupGet(x => x.Configuration).Returns(this.TestClientConfiguration);
			this.MockTransferClient.SetupGet(x => x.Id).Returns(this.TestClientConfiguration.ClientId);
			this.MockTransferClient.Setup(
					x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(this.MockTransferJob.Object);
			this.MockRelativityTransferHost = new Mock<IRelativityTransferHost>();
			this.MockRelativityTransferHost.Setup(x => x.CreateClient(It.IsAny<ClientConfiguration>()))
				.Returns(this.MockTransferClient.Object);
			this.MockRelativityTransferHost
				.Setup(x => x.CreateClientAsync(It.IsAny<ClientConfiguration>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(this.MockTransferClient.Object);
			this.MockRelativityTransferHost
				.Setup(
					x => x.CreateClientAsync(
						It.IsAny<ClientConfiguration>(),
						It.IsAny<ITransferClientStrategy>(),
						It.IsAny<CancellationToken>())).ReturnsAsync(this.MockTransferClient.Object);
			this.MockTapiObjectService = new Mock<ITapiObjectService>();
			this.MockTapiObjectService.Setup(
					x => x.CreateRelativityTransferHost(It.IsAny<RelativityConnectionInfo>(), It.IsAny<ITransferLog>()))
				.Returns(this.MockRelativityTransferHost.Object);
			this.MockTapiObjectService.Setup(x => x.CreateRelativityConnectionInfo(It.IsAny<TapiBridgeParameters>()))
				.Returns(new RelativityConnectionInfo());
			this.MockTapiObjectService.Setup(x => x.CreateFileSystemService())
				.Returns(this.MockTransferFileSystemService.Object);
			this.MockTapiObjectService.Setup(x => x.GetClientId(It.IsAny<TapiBridgeParameters>())).Returns(
				(TapiBridgeParameters parameters) => this.realObjectService.GetClientId(parameters));
			this.MockTapiObjectService.Setup(x => x.GetTapiClient(It.IsAny<Guid>()))
				.Returns((Guid id) => this.realObjectService.GetTapiClient(id));
			this.MockTransferLogger = new Mock<ITransferLog>();
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			if (this.TapiBridgeInstance != null)
			{
				this.TapiBridgeInstance.Dispose();
				this.TapiBridgeInstance = null;
			}

			this.OnTeardown();
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldDefaultTheTapiProperties()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);
			Assert.That(this.TapiBridgeInstance.Client, Is.EqualTo(TapiClient.None));
			Assert.That(this.TapiBridgeInstance.ClientId, Is.EqualTo(Guid.Empty));
			Assert.That(this.TapiBridgeInstance.TargetPath, Is.Null.Or.Empty);
		}

		[Test]
		[Category(TestCategories.TransferApi)]
		public void ShouldCreateTheTransferClientAsynchronously()
		{
			this.CreateTapiBridge(WellKnownTransferClient.Unassigned);

			// Adding a transfer path is expected to construct a transfer host, client, and job.
			this.TapiBridgeInstance.AddPath(new TransferPath());
			this.MockTapiObjectService.Verify(
				x => x.CreateRelativityTransferHost(It.IsAny<RelativityConnectionInfo>(), It.IsAny<ITransferLog>()));
			this.MockRelativityTransferHost.Verify(
				x => x.CreateClientAsync(
					It.IsAny<ClientConfiguration>(),
					It.IsAny<ITransferClientStrategy>(),
					It.IsAny<CancellationToken>()));
			this.MockTransferClient.Verify(
				x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()));
		}

		[Test]
		[TestCase(WellKnownTransferClient.FileShare)]
		[TestCase(WellKnownTransferClient.Aspera)]
		[TestCase(WellKnownTransferClient.Http)]
		[Category(TestCategories.TransferApi)]
		public void ShouldCreateTheTransferClientSynchronously(WellKnownTransferClient client)
		{
			this.CreateTapiBridge(client);

			// Adding a transfer path is expected to perform the following:
			// 1. Construct a transfer host
			// 2. Construct a transfer client
			// 3. Construct a transfer job
			this.TapiBridgeInstance.AddPath(new TransferPath());
			this.MockTapiObjectService.Verify(
				x => x.CreateRelativityTransferHost(It.IsAny<RelativityConnectionInfo>(), It.IsAny<ITransferLog>()));
			this.MockRelativityTransferHost.Verify(x => x.CreateClient(It.IsAny<ClientConfiguration>()));
			this.MockTransferClient.Verify(
				x => x.CreateJobAsync(It.IsAny<ITransferRequest>(), It.IsAny<CancellationToken>()));
		}

		protected abstract void CreateTapiBridge(WellKnownTransferClient client);

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnTeardown()
		{
		}

		protected TapiBridgeParameters CreateTapiBridgeParameters(WellKnownTransferClient client)
		{
			TapiBridgeParameters parameters = new TapiBridgeParameters
				                                  {
					                                  Credentials = new NetworkCredential(),
					                                  WebServiceUrl = "https://relativity.one.com",
					                                  WorkspaceId = this.TestWorkspaceId,
					                                  ForceAsperaClient = false,
					                                  ForceHttpClient = false,
					                                  ForceFileShareClient = false
				                                  };
			this.ConfigureTapiBridgeParameters(parameters, client);
			return parameters;
		}

		protected void ConfigureTapiBridgeParameters(TapiBridgeParameters parameters, WellKnownTransferClient client)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			this.TestClientConfiguration.Client = client;
			switch (client)
			{
				case WellKnownTransferClient.Aspera:
					parameters.ForceAsperaClient = true;
					break;

				case WellKnownTransferClient.FileShare:
					parameters.ForceFileShareClient = true;
					break;

				case WellKnownTransferClient.Http:
					parameters.ForceHttpClient = true;
					break;
			}
		}
	}
}
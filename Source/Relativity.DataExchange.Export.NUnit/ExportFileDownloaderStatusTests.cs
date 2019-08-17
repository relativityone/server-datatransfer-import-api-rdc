// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileDownloaderStatusTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	[TestFixture]
	public class ExportFileDownloaderStatusTests
	{
		private ExportFileDownloaderStatus _instance;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._tapiBridge = new Mock<ITapiBridge>();
			this._instance = new ExportFileDownloaderStatus(new NullLogger());
		}

		[Test]
		[TestCase("Aspera", TapiClient.Aspera)]
		[TestCase("aspera", TapiClient.Aspera)]
		[TestCase("Direct", TapiClient.Direct)]
		[TestCase("direct", TapiClient.Direct)]
		[TestCase("Web", TapiClient.Web)]
		[TestCase("web", TapiClient.Web)]
		[TestCase("Aspera/Web", TapiClient.Aspera | TapiClient.Web)]
		[TestCase("aspera/web", TapiClient.Aspera | TapiClient.Web)]
		[TestCase("Direct/Web", TapiClient.Direct | TapiClient.Web)]
		[TestCase("direct/web", TapiClient.Direct | TapiClient.Web)]
		[TestCase("Direct/Aspera/Web", TapiClient.Direct | TapiClient.Aspera | TapiClient.Web)]
		[TestCase("direct/aspera/web", TapiClient.Direct | TapiClient.Aspera | TapiClient.Web)]
		public void ItShouldUpdateTransferMode(string clientName, TapiClient expectedTapiClient)
		{
			TapiClient currentTapiClient = TapiClient.None;

			this._instance.Attach(this._tapiBridge.Object);
			this._instance.TransferModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT
			this._tapiBridge.Raise(
				x => x.TapiClientChanged += null,
				new TapiClientEventArgs(Guid.Empty, clientName, expectedTapiClient));

			// ASSERT
			Assert.That(currentTapiClient, Is.EqualTo(expectedTapiClient));
			Assert.That(this._instance.TransferMode, Is.EqualTo(expectedTapiClient));
		}

		[Test]
		public void ItShouldDetach()
		{
			const string aspera = "Aspera";
			const string web = "Web";
			TapiClient currentTapiClient = TapiClient.None;

			this._instance.Attach(this._tapiBridge.Object);
			this._instance.TransferModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT - attempting to detach an already detached bridge is safe.
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(Guid.Empty, aspera, TapiClient.Aspera));
			this._instance.Detach(this._tapiBridge.Object);
			this._instance.Detach(this._tapiBridge.Object);
			this._instance.Detach(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(Guid.Empty, web, TapiClient.Web));

			// ASSERT - detaching effectively clears the mode.
			Assert.That(currentTapiClient, Is.EqualTo(TapiClient.Aspera));
			Assert.That(this._instance.TransferMode, Is.EqualTo(TapiClient.None));
		}
	}
}
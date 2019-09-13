// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileDownloaderStatusTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
    using System.Collections.Generic;
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
		[TestCase("Aspera/Web", TapiClient.Aspera, TapiClient.Web)]
		[TestCase("aspera/web", TapiClient.Web, TapiClient.Aspera)]
		[TestCase("Direct/Web", TapiClient.Direct, TapiClient.Web)]
		[TestCase("direct/web", TapiClient.Web, TapiClient.Direct)]
		[TestCase("Direct/Aspera/Web", TapiClient.Direct, TapiClient.Aspera, TapiClient.Web)]
		[TestCase("direct/aspera/web", TapiClient.Direct, TapiClient.Aspera, TapiClient.Web)]
		public void ItShouldUpdateTransferMode(string clientName, params TapiClient[] expectedTapiClients)
		{
			List<TapiClient> currentTapiClients = new List<TapiClient>();
			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.TransferModesChangeEvent += (sender, args) =>
				{
					currentTapiClients.AddRange(args.TransferClients);
				};

			// ACT
			if (expectedTapiClients != null)
			{
				foreach (TapiClient client in expectedTapiClients)
				{
					this._tapiBridge.Raise(
						x => x.TapiClientChanged += null,
						new TapiClientEventArgs(Guid.Empty, clientName, client));
				}
			}

			// ASSERT
			Assert.That(currentTapiClients, Is.EquivalentTo(expectedTapiClients));
		}

		[Test]
		public void ItShouldDetach()
		{
			const string aspera = "Aspera";
			const string web = "Web";
			List<TapiClient> currentTapiClients = new List<TapiClient>();
			TapiClient currentTapiClient = TapiClient.None;

			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.TransferModesChangeEvent += (sender, args) =>
				{
					currentTapiClients.Clear();
					currentTapiClients.AddRange(args.TransferClients);
				};

			// ACT - attempting to detach an already detached bridge is safe.
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(Guid.Empty, aspera, TapiClient.Aspera));
			this._instance.Unsubscribe(this._tapiBridge.Object);
			this._instance.Unsubscribe(this._tapiBridge.Object);
			this._instance.Unsubscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(Guid.Empty, web, TapiClient.Web));

			// ASSERT - detaching effectively clears the mode.
			Assert.That(currentTapiClient, Is.EqualTo(TapiClient.None));
			Assert.That(this._instance.TransferModes.Count, Is.Zero);
		}
	}
}
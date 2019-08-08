// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileDownloaderStatusTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
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
		public void ItShouldUpdateUploaderType(string clientName, TapiClient expectedTapiClient)
		{
			TapiClient currentTapiClient = TapiClient.None;

			this._instance.Attach(this._tapiBridge.Object);
			this._instance.UploadModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(clientName, expectedTapiClient));

			// ASSERT
			Assert.That(currentTapiClient, Is.EqualTo(expectedTapiClient));
			Assert.That(this._instance.UploaderType, Is.EqualTo(expectedTapiClient));
		}

		[Test]
		public void ItShouldDetach()
		{
			const string aspera = "Aspera";
			const string web = "Web";
			TapiClient currentTapiClient = TapiClient.None;

			this._instance.Attach(this._tapiBridge.Object);
			this._instance.UploadModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT
			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(aspera, TapiClient.Aspera));

			this._instance.Detach(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(web, TapiClient.Web));

			// ASSERT
			Assert.That(currentTapiClient, Is.EqualTo(TapiClient.Aspera));
			Assert.That(this._instance.UploaderType, Is.EqualTo(TapiClient.Aspera));
		}
	}
}
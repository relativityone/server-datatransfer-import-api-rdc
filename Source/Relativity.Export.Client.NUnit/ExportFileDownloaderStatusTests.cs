// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileDownloaderStatusTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Export.VolumeManagerV2.Statistics;
	using Relativity.Import.Export.Transfer;
	using Relativity.Logging;

    [TestFixture]
	public class ExportFileDownloaderStatusTests
	{
		private ExportFileDownloaderStatus _instance;

		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = new ExportFileDownloaderStatus(new NullLogger());
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

			_instance.Attach(_tapiBridge.Object);
			_instance.UploadModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT
			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(clientName, expectedTapiClient));

			// ASSERT
			Assert.That(currentTapiClient, Is.EqualTo(expectedTapiClient));
			Assert.That(_instance.UploaderType, Is.EqualTo(expectedTapiClient));
		}

		[Test]
		public void ItShouldDetach()
		{
			const string aspera = "Aspera";
			const string web = "Web";
			TapiClient currentTapiClient = TapiClient.None;

			_instance.Attach(_tapiBridge.Object);
			_instance.UploadModeChangeEvent += newTapiClient =>
				{
					currentTapiClient = newTapiClient;
				};

			// ACT
			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(aspera, TapiClient.Aspera));

			_instance.Detach();

			_tapiBridge.Raise(x => x.TapiClientChanged += null, new TapiClientEventArgs(web, TapiClient.Web));

			// ASSERT
			Assert.That(currentTapiClient, Is.EqualTo(TapiClient.Aspera));
			Assert.That(_instance.UploaderType, Is.EqualTo(TapiClient.Aspera));
		}
	}
}
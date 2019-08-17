// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiObjectServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="TapiObjectService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="TapiObjectService"/> tests.
	/// </summary>
	[TestFixture]
	public class TapiObjectServiceTests
	{
		private ITapiObjectService service;

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			this.service = new TapiObjectService();
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheFileTransferModeDocText(bool includeBulk)
		{
			string text = this.service.BuildFileTransferModeDocText(includeBulk);
			Assert.That(text, Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCase(null, null, "Native: Disabled, Metadata: Disabled")]
		[TestCase(null, TapiClient.Web, "Native: Disabled, Metadata: Web")]
		[TestCase(TapiClient.None, TapiClient.None, "Native: Pending, Metadata: Pending")]
		[TestCase(TapiClient.Aspera, TapiClient.Aspera, "Native: Aspera, Metadata: Aspera")]
		[TestCase(TapiClient.Aspera | TapiClient.None, TapiClient.Web | TapiClient.None, "Native: Aspera, Metadata: Web")]
		[TestCase(TapiClient.Aspera | TapiClient.Web, TapiClient.Aspera | TapiClient.Web, "Native: Aspera/Web, Metadata: Aspera/Web")]
		[TestCase(TapiClient.Direct | TapiClient.Aspera, TapiClient.Aspera | TapiClient.Direct, "Native: Direct/Aspera, Metadata: Direct/Aspera")]
		[TestCase(TapiClient.Web | TapiClient.Aspera, TapiClient.Aspera, "Native: Aspera/Web, Metadata: Aspera")]
		[TestCase(TapiClient.Web | TapiClient.Aspera | TapiClient.Direct, TapiClient.Web, "Native: Direct/Aspera/Web, Metadata: Web")]
		[TestCase(TapiClient.Web | TapiClient.Aspera | TapiClient.Direct, TapiClient.Web | TapiClient.Direct, "Native: Direct/Aspera/Web, Metadata: Direct/Web")]
		[TestCase(TapiClient.Web | TapiClient.Aspera | TapiClient.Direct, TapiClient.Web | TapiClient.Aspera | TapiClient.Direct, "Native: Direct/Aspera/Web, Metadata: Direct/Aspera/Web")]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheFileTransferModeStatusBarText(TapiClient? native, TapiClient? metadata, string expected)
		{
			string text = this.service.BuildFileTransferModeStatusBarText(native, metadata);
			Assert.That(text, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(TransferClientConstants.AsperaClientId, "Aspera")]
		[TestCase(TransferClientConstants.FileShareClientId, "Direct")]
		[TestCase(TransferClientConstants.HttpClientId, "Web")]
		[TestCase("00000000-0000-0000-0000-000000000000", null)]
		[TestCase("E153A87D-0C4F-4561-B43A-8576F97C2A01", null)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldGetTheClientDisplayName(string clientId, string expected)
		{
			if (!string.IsNullOrEmpty(expected))
			{
				string name = this.service.GetClientDisplayName(new Guid(clientId));
				Assert.That(name, Is.EqualTo(expected));
			}
			else
			{
				Assert.Throws<ArgumentException>(() => this.service.GetClientDisplayName(new Guid(clientId)));
			}
		}

		[Test]
		[TestCase(TransferClientConstants.AsperaClientId, TapiClient.Aspera)]
		[TestCase(TransferClientConstants.FileShareClientId, TapiClient.Direct)]
		[TestCase(TransferClientConstants.HttpClientId, TapiClient.Web)]
		[TestCase("00000000-0000-0000-0000-000000000000", TapiClient.None)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldGetTheTapiClient(string clientId, TapiClient expected)
		{
			TapiClient client = this.service.GetTapiClient(new Guid(clientId));
			Assert.That(client, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(TapiClient.Aspera, true, false, false)]
		[TestCase(TapiClient.Direct, false, true, false)]
		[TestCase(TapiClient.Web, false, false, true)]
		[TestCase(TapiClient.None, false, false, false)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldSetTheTapiClient(
			TapiClient client,
			bool expectedForceAsperaClient,
			bool expectedForceFileShareClient,
			bool expectedForceHttpClient)
		{
			TapiBridgeParameters2 parameters = new TapiBridgeParameters2();
			this.service.SetTapiClient(parameters, client);
			Assert.That(parameters.ForceAsperaClient, Is.EqualTo(expectedForceAsperaClient));
			Assert.That(parameters.ForceFileShareClient, Is.EqualTo(expectedForceFileShareClient));
			Assert.That(parameters.ForceHttpClient, Is.EqualTo(expectedForceHttpClient));
		}

		[Test]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldApplyTheUnmappedFileRepositoryParameters()
		{
			TapiBridgeParameters2 parameters = new TapiBridgeParameters2
				                                   {
					                                   ForceAsperaClient = true,
					                                   ForceFileShareClient = true,
					                                   ForceHttpClient = true,
					                                   ForceClientCandidates = "xyz"
				                                   };
			this.service.ApplyUnmappedFileRepositoryParameters(parameters);
			Assert.That(parameters.ForceAsperaClient, Is.False);
			Assert.That(parameters.ForceFileShareClient, Is.True);
			Assert.That(parameters.ForceHttpClient, Is.True);
			Assert.That(parameters.ForceClientCandidates, Is.Not.Null.Or.Empty);
			string[] clientList = parameters.ForceClientCandidates.Split(';');
			Assert.That(clientList.Length, Is.EqualTo(2));
			Assert.That(
				clientList,
				Is.EquivalentTo(
					new[] { WellKnownTransferClient.FileShare.ToString(), WellKnownTransferClient.Http.ToString() }));
		}
	}
}
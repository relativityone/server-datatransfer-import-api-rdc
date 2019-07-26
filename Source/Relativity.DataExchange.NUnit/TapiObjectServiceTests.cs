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
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheFileTransferModeDocText(bool includeBulk)
		{
			string text = this.service.BuildFileTransferModeDocText(includeBulk);
			Assert.That(text, Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCase(TransferClientConstants.AsperaClientId, true)]
		[TestCase(TransferClientConstants.FileShareClientId, true)]
		[TestCase(TransferClientConstants.HttpClientId, true)]
		[TestCase("00000000-0000-0000-0000-000000000000", false)]
		[TestCase("E153A87D-0C4F-4561-B43A-8576F97C2A01", false)]
		[Category(TestCategories.TransferApi)]
		public void ShouldGetTheClientDisplayName(string clientId, bool expected)
		{
			if (expected)
			{
				string name = this.service.GetClientDisplayName(new Guid(clientId));
				Assert.That(name, Is.Not.Null.Or.Empty);
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
		[Category(TestCategories.TransferApi)]
		public void ShouldGetTheTapiClient(string clientId, TapiClient expected)
		{
			TapiClient client = this.service.GetTapiClient(new Guid(clientId));
			Assert.That(client, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(TapiClient.Aspera)]
		[TestCase(TapiClient.Direct)]
		[TestCase(TapiClient.Web)]
		[TestCase(TapiClient.None)]
		[Category(TestCategories.TransferApi)]
		public void ShouldSetTheTapiClient(TapiClient client)
		{
			TapiBridgeParameters2 parameters = new TapiBridgeParameters2();
			this.service.SetTapiClient(parameters, client);
			Assert.That(parameters.ForceAsperaClient, Is.EqualTo(client == TapiClient.Aspera));
			Assert.That(parameters.ForceFileShareClient, Is.EqualTo(client == TapiClient.Direct));
			Assert.That(parameters.ForceHttpClient, Is.EqualTo(client == TapiClient.Web));
		}

		[Test]
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
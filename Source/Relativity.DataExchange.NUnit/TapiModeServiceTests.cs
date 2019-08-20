// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiModeServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="TapiModeService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	/// <summary>
	/// Represents <see cref="TapiModeService"/> tests.
	/// </summary>
	[TestFixture]
	public class TapiModeServiceTests
	{
		private TapiModeService service;

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			this.service = new TapiModeService();
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheDocText(bool includeBulk)
		{
			string text = this.service.BuildDocText(includeBulk);
			Assert.That(text, Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCase("Native: Disabled, Metadata: Pending", false, null, TapiClient.None)]
		[TestCase("Native: Disabled, Metadata: Web", false, null, TapiClient.Web)]
		[TestCase("Native: Pending, Metadata: Pending", true, TapiClient.None, TapiClient.None)]
		[TestCase("Native: Pending, Metadata: Pending", true, null, null)]
		[TestCase("Native: Aspera, Metadata: Direct", true, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("Native: Aspera, Metadata: Aspera", true, TapiClient.Aspera, TapiClient.Aspera)]
		[TestCase("Native: Aspera, Metadata: Web", true, TapiClient.Aspera, TapiClient.Web)]
		[TestCase("Native: Direct, Metadata: Direct", true, TapiClient.Direct, TapiClient.Direct)]
		[TestCase("Native: Direct, Metadata: Aspera", true, TapiClient.Direct, TapiClient.Aspera)]
		[TestCase("Native: Direct, Metadata: Web", true, TapiClient.Direct, TapiClient.Web)]
		[TestCase("Native: Web, Metadata: Direct", true, TapiClient.Web, TapiClient.Direct)]
		[TestCase("Native: Web, Metadata: Aspera", true, TapiClient.Web, TapiClient.Aspera)]
		[TestCase("Native: Web, Metadata: Web", true, TapiClient.Web, TapiClient.Web)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheImportStatusText(
			string expected,
			bool nativeFilesCopied,
			TapiClient? native,
			TapiClient? metadata)
		{
			string text = this.service.BuildImportStatusText(nativeFilesCopied, native, metadata);
			Assert.That(text, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("Native: Pending", TapiClient.None)]
		[TestCase("Native: Aspera", TapiClient.Aspera)]
		[TestCase("Native: Aspera/Web", TapiClient.Aspera, TapiClient.Web)]
		[TestCase("Native: Direct/Aspera", TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("Native: Direct/Aspera", TapiClient.Direct, TapiClient.Aspera)]
		[TestCase("Native: Aspera/Web", TapiClient.Web, TapiClient.Aspera)]
		[TestCase("Native: Aspera/Web", TapiClient.Aspera, TapiClient.Web)]
		[TestCase("Native: Direct/Aspera/Web", TapiClient.Web, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("Native: Direct/Aspera/Web", TapiClient.Direct, TapiClient.Web, TapiClient.Aspera)]
		[TestCase("Native: Direct/Aspera/Web", TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheExportStatusText(string expected, params TapiClient[] natives)
		{
			string text = this.service.BuildExportStatusText(natives);
			Assert.That(text, Is.EqualTo(expected));
		}
	}
}
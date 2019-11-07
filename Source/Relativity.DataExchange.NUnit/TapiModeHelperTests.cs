﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiModeHelperTests.cs" company="Relativity ODA LLC">
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
	/// Represents <see cref="TapiModeHelper"/> tests.
	/// </summary>
	[TestFixture]
	public class TapiModeHelperTests
	{
		[Test]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheDocText()
		{
			string text = TapiModeHelper.BuildDocText();
			Assert.That(text, Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCase("File Transfer Mode: Disabled, Metadata: Pending", false, null, TapiClient.None)]
		[TestCase("File Transfer Mode: Disabled, Metadata: Web", false, null, TapiClient.Web)]
		[TestCase("File Transfer Mode: Pending, Metadata: Pending", true, TapiClient.None, TapiClient.None)]
		[TestCase("File Transfer Mode: Pending, Metadata: Pending", true, null, null)]
		[TestCase("File Transfer Mode: Aspera, Metadata: Direct", true, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("File Transfer Mode: Aspera, Metadata: Aspera", true, TapiClient.Aspera, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Aspera, Metadata: Web", true, TapiClient.Aspera, TapiClient.Web)]
		[TestCase("File Transfer Mode: Direct, Metadata: Direct", true, TapiClient.Direct, TapiClient.Direct)]
		[TestCase("File Transfer Mode: Direct, Metadata: Aspera", true, TapiClient.Direct, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Direct, Metadata: Web", true, TapiClient.Direct, TapiClient.Web)]
		[TestCase("File Transfer Mode: Web, Metadata: Direct", true, TapiClient.Web, TapiClient.Direct)]
		[TestCase("File Transfer Mode: Web, Metadata: Aspera", true, TapiClient.Web, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Web, Metadata: Web", true, TapiClient.Web, TapiClient.Web)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheDocumentImportStatusText(
			string expected,
			bool nativeFilesCopied,
			TapiClient? native,
			TapiClient? metadata)
		{
			string documentImportStatusText = TapiModeHelper.BuildImportStatusText(nativeFilesCopied, native, metadata);
			Assert.That(documentImportStatusText, Is.EqualTo(expected));
			string imageImportStatusText = TapiModeHelper.BuildImportStatusText(nativeFilesCopied, native, metadata);
			Assert.That(imageImportStatusText, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("File Transfer Mode: Disabled", false, TapiClient.None)]
		[TestCase("File Transfer Mode: Pending", true, TapiClient.None)]
		[TestCase("File Transfer Mode: Aspera", true, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Aspera/Web", true, TapiClient.Aspera, TapiClient.Web)]
		[TestCase("File Transfer Mode: Direct/Aspera", true, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("File Transfer Mode: Direct/Aspera", true, TapiClient.Direct, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Aspera/Web", true, TapiClient.Web, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Aspera/Web", true, TapiClient.Aspera, TapiClient.Web)]
		[TestCase("File Transfer Mode: Direct/Aspera/Web", true, TapiClient.Web, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase("File Transfer Mode: Direct/Aspera/Web", true, TapiClient.Direct, TapiClient.Web, TapiClient.Aspera)]
		[TestCase("File Transfer Mode: Direct/Aspera/Web", true, TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldBuildTheExportStatusText(string expected, bool nativeFilesCopied, params TapiClient[] natives)
		{
			string text = TapiModeHelper.BuildExportStatusText(nativeFilesCopied, natives);
			Assert.That(text, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(TapiClient.None)]
		[TestCase(TapiClient.Direct, TapiClient.None, TapiClient.Direct, TapiClient.None)]
		[TestCase(TapiClient.Aspera, TapiClient.Direct, TapiClient.Aspera, TapiClient.Direct)]
		[TestCase(TapiClient.Web, TapiClient.Web, TapiClient.None, TapiClient.Aspera)]
		[TestCase(TapiClient.Aspera, TapiClient.Aspera)]
		[Repeat(3)]
		[Category(TestCategories.TransferApi)]
		public void ShouldGetCorrectTapiClient(TapiClient expected, params TapiClient[] clients)
		{
			var actual = TapiModeHelper.GetTapiClient(clients);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
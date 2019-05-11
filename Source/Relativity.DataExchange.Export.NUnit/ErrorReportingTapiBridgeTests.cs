// -----------------------------------------------------------------------------------------------------
// <copyright file="ErrorReportingTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Transfer;

	[TestFixture]
	public class ErrorReportingTapiBridgeTests
	{
		[Test]
		public void ItShouldRaiseUnsuccessfulTapiProgressEvent()
		{
			string actualFileName = null;
			bool actualDidTransferSucceed = true;

			var uut = new ErrorReportingTapiBridge();
			uut.TapiProgress += (s, e) =>
			{
				actualFileName = e.FileName;
				actualDidTransferSucceed = e.DidTransferSucceed;
			};

			TransferPath path = new TransferPath { SourcePath = @"\\test\path\source.txt" };
			uut.AddPath(path);

			Assert.IsNotNull(actualFileName);
			Assert.IsFalse(actualDidTransferSucceed);
		}

		[TestCase(@"\\share\location\test.html", "test_out.html", "test_out.html")]
		[TestCase(@"\\share\location\test.html", "", "test.html")]
		[TestCase(@"\\share\location\test.html", null, "test.html")]
		public void ItShouldRaiseTapiProgressEventWithCorrectFileName(string sourcePath, string targetFileName, string expectedFileName)
		{
			string actualFileName = null;

			var uut = new ErrorReportingTapiBridge();
			uut.TapiProgress += (s, e) =>
			{
				actualFileName = e.FileName;
			};

			TransferPath path = new TransferPath { SourcePath = sourcePath, TargetFileName = targetFileName };
			uut.AddPath(path);

			Assert.AreEqual(expectedFileName, actualFileName);
		}
	}
}

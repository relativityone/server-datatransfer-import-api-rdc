// -----------------------------------------------------------------------------------------------------
// <copyright file="ErrorFileDestinationPathTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Logging;

	[TestFixture]
	public class ErrorFileDestinationPathTests
	{
		private ErrorFileDestinationPath _instance;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1);

			this._instance = new ErrorFileDestinationPath(exportSettings, new NullLogger());
		}

		[Test]
		public void ItShouldReturnSamePathEveryTime()
		{
			// ACT
			string path1 = this._instance.Path;
			string path2 = this._instance.Path;

			// ASSERT
			Assert.That(path1, Is.Not.Null.Or.Empty);
			Assert.That(path1, Is.EqualTo(path2));
		}

		[Test]
		public void ItShouldReturnItErrorFileCreated()
		{
			// ACT
			bool beforeFirstPathCall = this._instance.IsErrorFileCreated();
			bool afterFirstPathCall = this._instance.IsErrorFileCreated();

			// ASSERT
			Assert.That(beforeFirstPathCall, Is.False);
			Assert.That(afterFirstPathCall, Is.True);
		}
	}
}
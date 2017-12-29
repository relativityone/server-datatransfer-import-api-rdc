using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata
{
	[TestFixture]
	public class ErrorFileDestinationPathTests
	{
		private ErrorFileDestinationPath _instance;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1);

			_instance = new ErrorFileDestinationPath(exportSettings, new NullLogger());
		}

		[Test]
		public void ItShouldReturnSamePathEveryTime()
		{
			//ACT
			string path1 = _instance.Path;
			string path2 = _instance.Path;

			//ASSERT
			Assert.That(path1, Is.Not.Null.Or.Empty);
			Assert.That(path1, Is.EqualTo(path2));
		}

		[Test]
		public void ItShouldReturnItErrorFileCreated()
		{
			//ACT
			bool beforeFirstPathCall = _instance.IsErrorFileCreated();
			string path = _instance.Path;
			bool afterFirstPathCall = _instance.IsErrorFileCreated();

			//ASSERT
			Assert.That(beforeFirstPathCall, Is.False);
			Assert.That(afterFirstPathCall, Is.True);
		}
	}
}
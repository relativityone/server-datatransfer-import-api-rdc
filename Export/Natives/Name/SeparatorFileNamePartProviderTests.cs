
using kCura.WinEDDS.Core.Export.Natives.Name;
using kCura.WinEDDS.Core.Model;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class SeparatorFileNamePartProviderTests
	{
		private SeparatorFileNamePartProvider _subjectUnderTests;

		[Test]
		public void ItShouldReturnCorrectSeparator()
		{
			var expectedSep = "_";

			_subjectUnderTests = new SeparatorFileNamePartProvider();

			string retSep = _subjectUnderTests.GetPartName(new SeparatorDescriptorPart(expectedSep), null);

			Assert.That(retSep, Is.EqualTo(expectedSep));
		}
	}
}

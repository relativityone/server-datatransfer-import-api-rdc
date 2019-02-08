using FileNaming.CustomFileNaming;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class CustomTextFileNamePartProviderTests
	{
		private CustomTextFileNamePartProvider _subjectUnderTests;

		[Test]
		public void ItShouldReturnCorrectSeparator()
		{
			const string expectedSep = "CustomText";

			_subjectUnderTests = new CustomTextFileNamePartProvider();

			string retSep = _subjectUnderTests.GetPartName(new CustomTextDescriptorPart(expectedSep), null);

			Assert.That(retSep, Is.EqualTo(expectedSep));
		}
	}
}

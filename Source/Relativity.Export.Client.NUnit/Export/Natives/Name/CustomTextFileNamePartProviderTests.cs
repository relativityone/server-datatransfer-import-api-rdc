// ----------------------------------------------------------------------------
// <copyright file="CustomTextFileNamePartProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export.Natives.Name
{
    using FileNaming.CustomFileNaming;

    using kCura.WinEDDS.FileNaming.CustomFileNaming;

    using global::NUnit.Framework;

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

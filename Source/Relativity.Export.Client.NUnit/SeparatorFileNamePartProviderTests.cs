// -----------------------------------------------------------------------------------------------------
// <copyright file="SeparatorFileNamePartProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using FileNaming.CustomFileNaming;

    using kCura.WinEDDS.FileNaming.CustomFileNaming;

    using global::NUnit.Framework;

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

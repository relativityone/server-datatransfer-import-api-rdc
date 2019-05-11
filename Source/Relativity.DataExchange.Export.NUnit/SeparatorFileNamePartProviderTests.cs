// -----------------------------------------------------------------------------------------------------
// <copyright file="SeparatorFileNamePartProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using FileNaming.CustomFileNaming;

	using global::NUnit.Framework;

	using kCura.WinEDDS.FileNaming.CustomFileNaming;

	public class SeparatorFileNamePartProviderTests
	{
		private SeparatorFileNamePartProvider _subjectUnderTests;

		[Test]
		public void ItShouldReturnCorrectSeparator()
		{
			var expectedSep = "_";

			this._subjectUnderTests = new SeparatorFileNamePartProvider();

			string retSep = this._subjectUnderTests.GetPartName(new SeparatorDescriptorPart(expectedSep), null);

			Assert.That(retSep, Is.EqualTo(expectedSep));
		}
	}
}

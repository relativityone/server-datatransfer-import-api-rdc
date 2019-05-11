﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="CustomTextFileNamePartProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using FileNaming.CustomFileNaming;

	using global::NUnit.Framework;

	using kCura.WinEDDS.FileNaming.CustomFileNaming;

	public class CustomTextFileNamePartProviderTests
	{
		private CustomTextFileNamePartProvider _subjectUnderTests;

		[Test]
		public void ItShouldReturnCorrectSeparator()
		{
			const string expectedSep = "CustomText";

			this._subjectUnderTests = new CustomTextFileNamePartProvider();

			string retSep = this._subjectUnderTests.GetPartName(new CustomTextDescriptorPart(expectedSep), null);

			Assert.That(retSep, Is.EqualTo(expectedSep));
		}
	}
}
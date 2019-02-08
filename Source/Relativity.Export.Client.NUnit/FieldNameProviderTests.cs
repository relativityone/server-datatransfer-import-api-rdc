// ----------------------------------------------------------------------------
// <copyright file="FieldNameProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using kCura.WinEDDS.Core.Export;

    using global::NUnit.Framework;

    public class FieldNameProviderTests
	{
		private FieldNameProvider _subjectUnderTest;

		[SetUp]
		public void Init()
		{
			_subjectUnderTest = new FieldNameProvider();
		}

		[Test]
		public void ItShouldReturnDisplayName()
		{
			string expectedText = "ColName";
			kCura.WinEDDS.ViewFieldInfo fieldInfo = ViewFieldInfoMockFactory.CreateMockedViewFieldInfoArray(new List<Tuple<int, string>>
			{
				Tuple.Create<int, string>(1, expectedText)
			}).FirstOrDefault();

			string retName = _subjectUnderTest.GetDisplayName(fieldInfo);

			Assert.That(retName, Is.EqualTo(expectedText));
		}
	}
}

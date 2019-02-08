using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export;
using kCura.WinEDDS.Core.NUnit.Helpers;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export
{
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
			ViewFieldInfo fieldInfo = ViewFieldInfoMockFactory.CreateMockedViewFieldInfoArray(new List<Tuple<int, string>>
			{
				Tuple.Create<int, string>(1, expectedText)
			}).FirstOrDefault();

			string retName = _subjectUnderTest.GetDisplayName(fieldInfo);

			Assert.That(retName, Is.EqualTo(expectedText));
		}
	}
}

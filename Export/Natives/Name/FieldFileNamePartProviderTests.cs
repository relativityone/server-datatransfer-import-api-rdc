
using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.Natives.Name;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Core.NUnit.Helpers;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class FieldFileNamePartProviderTests
	{
		private FieldFileNamePartProvider _subjectUnderTest;
		private ExtendedObjectExportInfo _extendedObjectExportInfo;
		private const int _AVF_ID = 1;


		private ViewFieldInfoMockFactory _fieldInfoMockFactory = new ViewFieldInfoMockFactory();

		[SetUp]
		public void SetUp()
		{
			_subjectUnderTest = new FieldFileNamePartProvider();
			_extendedObjectExportInfo = new ExtendedObjectExportInfo();
			
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnDateFieldValue()
		{
			var fieldValue = new DateTime(2020, 1, 2, 10, 3, 5);

			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory.Build().WithAvfId(_AVF_ID).Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue
			};

			// assumption: one column selected by user (Control Number)
			_extendedObjectExportInfo.SelectedViewFieldsCount = 1;
			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo("01_02_2020 10_03_05 AM"));
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnChoiceFieldValue()
		{
			var fieldValue = "Some Custodian";

			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithFieldType(FieldTypeHelper.FieldType.Code)
				.Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue
			};

			// assumption: one column selected by user (Control Number)
			_extendedObjectExportInfo.SelectedViewFieldsCount = 1;
			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo(fieldValue));
		}
	}
}

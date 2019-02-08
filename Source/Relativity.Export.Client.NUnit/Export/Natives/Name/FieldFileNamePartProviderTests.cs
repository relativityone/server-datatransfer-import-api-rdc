﻿using System;
using System.Collections.Generic;
using FileNaming.CustomFileNaming;
using kCura.WinEDDS.Core.NUnit.Helpers;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class FieldFileNamePartProviderTests
	{
		private FieldFileNamePartProvider _subjectUnderTest;
		private ExtendedObjectExportInfo _extendedObjectExportInfo;
		private Mock<IFieldLookupService> _fieldLookupServiceMock;

		private const int _AVF_ID = 1;
		private const string _NAME = "ColName";
		private const int _COL_INDEX = 1;

		private readonly ViewFieldInfoMockFactory _fieldInfoMockFactory = new ViewFieldInfoMockFactory();

		[SetUp]
		public void SetUp()
		{
			_fieldLookupServiceMock = new Mock<IFieldLookupService>();
			_subjectUnderTest = new FieldFileNamePartProvider();

			_fieldLookupServiceMock.Setup(item => item.GetOrdinalIndex(_NAME)).Returns(_COL_INDEX);

			_extendedObjectExportInfo = new ExtendedObjectExportInfo(_fieldLookupServiceMock.Object);
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnDateFieldValue()
		{
			var fieldValue = new DateTime(2020, 1, 2, 10, 3, 5);

			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldTypeHelper.FieldType.Date)
				.WithFormatString("o")
				.Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue //_COL_INDEX
			};

			// assumption: one column selected by user (Control Number)
			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo(fieldValue.ToString("o").Replace(":", "_")));
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnChoiceFieldValue()
		{
			var fieldValue = "Some Custodian";

			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldTypeHelper.FieldType.Code)
				.Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue //_COL_INDEX
			};

			// assumption: one column selected by user (Control Number)
			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo(fieldValue));
		}

		[Test]
		[TestCase("True", "Has Native")]
		[TestCase("False", "")]
		public void ItShouldReturnFieldDisplayTextWhenGivenBooleanField(String fieldValue, String expectedValue)
		{

			string displayName = "Has Native";
			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldTypeHelper.FieldType.Boolean)
				.WithDisplayName(displayName)
				.Create();

			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue 
			};

			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.AreEqual(expectedValue, retFieldValue);
		}

		[Test]
		public void ItShouldClearVarcharFromObjectTags()
		{
			string fieldValue = "<object/><object/>QQ";
			string displayName = "Production::Begin Bates";
			string expectedVal = "QQ";
			ViewFieldInfo viewFieldInfo = _fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldTypeHelper.FieldType.Varchar)
				.WithDisplayName(displayName)
				.Create();

			_extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue
			};

			_extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = _subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);

			Assert.AreEqual(expectedVal, retFieldValue);
		}

		[Test]
		public void ItShouldThrowExceptionWhenSelectedNativeFileNameViewFieldsIsEmpty()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				_subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), _extendedObjectExportInfo);
			});
		}
	}
}

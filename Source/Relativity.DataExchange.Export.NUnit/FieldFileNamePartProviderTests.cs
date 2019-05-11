﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="FieldFileNamePartProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;

	using FileNaming.CustomFileNaming;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.FileNaming.CustomFileNaming;

	using Moq;

	using Relativity.DataExchange.Service;

	public class FieldFileNamePartProviderTests
	{
		private const int _AVF_ID = 1;
		private const string _NAME = "ColName";
		private const int _COL_INDEX = 1;
		private readonly ViewFieldInfoMockFactory _fieldInfoMockFactory = new ViewFieldInfoMockFactory();
		private FieldFileNamePartProvider _subjectUnderTest;
		private ExtendedObjectExportInfo _extendedObjectExportInfo;
		private Mock<IFieldLookupService> _fieldLookupServiceMock;

		[SetUp]
		public void SetUp()
		{
			this._fieldLookupServiceMock = new Mock<IFieldLookupService>();
			this._subjectUnderTest = new FieldFileNamePartProvider();

			this._fieldLookupServiceMock.Setup(item => item.GetOrdinalIndex(_NAME)).Returns(_COL_INDEX);

			this._extendedObjectExportInfo = new ExtendedObjectExportInfo(this._fieldLookupServiceMock.Object);
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnDateFieldValue()
		{
			var fieldValue = new DateTime(2020, 1, 2, 10, 3, 5);

			kCura.WinEDDS.ViewFieldInfo viewFieldInfo = this._fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldType.Date)
				.WithFormatString("o")
				.Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			this._extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",

				// _COL_INDEX
				fieldValue
			};

			// assumption: one column selected by user (Control Number)
			this._extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<kCura.WinEDDS.ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = this._subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), this._extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo(fieldValue.ToString("o").Replace(":", "_")));
		}

		[Test]
		public void ItShouldReturnPartNameBasedOnChoiceFieldValue()
		{
			var fieldValue = "Some Custodian";

            kCura.WinEDDS.ViewFieldInfo viewFieldInfo = this._fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldType.Code)
				.Create();

			// first column represent col in load File, second column will refer to field value taken part in creation of native file name
			this._extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",

				// _COL_INDEX
				fieldValue
			};

			// assumption: one column selected by user (Control Number)
			this._extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<kCura.WinEDDS.ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = this._subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), this._extendedObjectExportInfo);

			Assert.That(retFieldValue, Is.EqualTo(fieldValue));
		}

		[Test]
		[TestCase("True", "Has Native")]
		[TestCase("False", "")]
		public void ItShouldReturnFieldDisplayTextWhenGivenBooleanField(string fieldValue, string expectedValue)
		{
			string displayName = "Has Native";
            kCura.WinEDDS.ViewFieldInfo viewFieldInfo = this._fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldType.Boolean)
				.WithDisplayName(displayName)
				.Create();

			this._extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue
			};

			this._extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<kCura.WinEDDS.ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = this._subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), this._extendedObjectExportInfo);

			Assert.AreEqual(expectedValue, retFieldValue);
		}

		[Test]
		public void ItShouldClearVarcharFromObjectTags()
		{
			string fieldValue = "<object/><object/>QQ";
			string displayName = "Production::Begin Bates";
			string expectedVal = "QQ";
            kCura.WinEDDS.ViewFieldInfo viewFieldInfo = this._fieldInfoMockFactory
				.Build()
				.WithAvfId(_AVF_ID)
				.WithAvfName(_NAME)
				.WithFieldType(FieldType.Varchar)
				.WithDisplayName(displayName)
				.Create();

			this._extendedObjectExportInfo.Metadata = new object[]
			{
				"Some Control Number",
				fieldValue
			};

			this._extendedObjectExportInfo.SelectedNativeFileNameViewFields = new List<kCura.WinEDDS.ViewFieldInfo>
			{
				viewFieldInfo
			};

			string retFieldValue = this._subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), this._extendedObjectExportInfo);

			Assert.AreEqual(expectedVal, retFieldValue);
		}

		[Test]
		public void ItShouldThrowExceptionWhenSelectedNativeFileNameViewFieldsIsEmpty()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this._subjectUnderTest.GetPartName(new FieldDescriptorPart(_AVF_ID), this._extendedObjectExportInfo);
			});
		}
	}
}
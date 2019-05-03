// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Linq;
    using System.Text;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Export.VolumeManagerV2.Repository;
    using Relativity.Import.Export.Service;
	using Relativity.Import.Export.TestFramework;
	using Relativity.Logging;

    [TestFixture]
	public class LongTextHelperTests
	{
		private LongTextHelper _instance;

		private ExportFile _exportSettings;
		private QueryFieldFactory _fieldFactory;

		private Mock<IFieldService> _fieldService;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1);
			_fieldFactory = new QueryFieldFactory();

			_fieldService = new Mock<IFieldService>();

			_instance = new LongTextHelper(_exportSettings, _fieldService.Object, new LongTextRepository(null, new NullLogger()));
		}

		[Test]
		[TestCase(FieldType.Text, true)]
		[TestCase(FieldType.OffTableText, true)]
		[TestCase(FieldType.Object, false)]
		[TestCase(FieldType.Boolean, false)]
		[TestCase(FieldType.Code, false)]
		[TestCase(FieldType.Currency, false)]
		[TestCase(FieldType.Date, false)]
		[TestCase(FieldType.Decimal, false)]
		[TestCase(FieldType.Empty, false)]
		[TestCase(FieldType.File, false)]
		[TestCase(FieldType.Integer, false)]
		[TestCase(FieldType.LayoutText, false)]
		[TestCase(FieldType.MultiCode, false)]
		[TestCase(FieldType.Objects, false)]
		[TestCase(FieldType.User, false)]
		[TestCase(FieldType.Varchar, false)]
		public void ItShouldDecideIfFieldIsLongText(FieldType fieldType, bool expectedResult)
		{
			FieldStub field = new FieldStub(_fieldFactory.GetArtifactIdField());
			field.SetType(fieldType);

			// ACT
			bool isFieldLongTextResult = _instance.IsLongTextField(field);
			bool isLongTextType = _instance.IsLongTextField(fieldType);

			// ASSERT
			Assert.That(isFieldLongTextResult, Is.EqualTo(expectedResult));
			Assert.That(isLongTextType, Is.EqualTo(expectedResult));
		}

		[Test]
		public void ItShouldReturnTextFromString()
		{
			const string expectedText = "expected_text";
			const string fieldName = "fieldName";

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { expectedText } };

			_fieldService.Setup(x => x.GetOrdinalIndex(fieldName)).Returns(0);

			// ACT
			string actualResult = _instance.GetTextFromField(artifact, fieldName);

			// ASSERT
			Assert.That(actualResult, Is.EqualTo(expectedText));
		}

		[Test]
		public void ItShouldReturnTextFromBinaryRepresentation()
		{
			const string expectedText = "expected_text_from_binary";
			const string fieldName = "fieldName";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { Encoding.Unicode.GetBytes(expectedText) }
			};

			_fieldService.Setup(x => x.GetOrdinalIndex(fieldName)).Returns(0);

			// ACT
			string actualResult = _instance.GetTextFromField(artifact, fieldName);

			// ASSERT
			Assert.That(actualResult, Is.EqualTo(expectedText));
		}

		[Test]
		public void ItShouldHandleMissingText()
		{
			const string expectedText = null;
			const string fieldName = "fieldName";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { expectedText }
			};

			_fieldService.Setup(x => x.GetOrdinalIndex(fieldName)).Returns(0);

			// ACT
			string actualResult = _instance.GetTextFromField(artifact, fieldName);

			// ASSERT
			Assert.That(actualResult, Is.EqualTo(string.Empty));
		}

		[Test]
		[TestCase("not too long text", false)]
		[TestCase("not too long text not too long text not too long text not too long text not too long text", false)]
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase(ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN, true)]
		public void ItShouldCheckIfLongTextIsTooLong(string text, bool expectedResult)
		{
			const string fieldName = "fieldName";

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { text } };

			_fieldService.Setup(x => x.GetOrdinalIndex(fieldName)).Returns(0);

			// ACT
			bool isTextTooLong = _instance.IsTextTooLong(text);
			bool isTextFromFieldTooLong = _instance.IsTextTooLong(artifact, fieldName);

			// ASSERT
			Assert.That(isTextTooLong, Is.EqualTo(expectedResult));
			Assert.That(isTextFromFieldTooLong, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(1, false)]
		[TestCase(5, true)]
		[TestCase(10, true)]
		public void ItShouldDecideIfExtractedTextFieldIsMissing(int extractedTextFieldIndex, bool expectedResult)
		{
			_fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(extractedTextFieldIndex);

			_fieldService.Setup(x => x.GetColumns()).Returns(new kCura.WinEDDS.ViewFieldInfo[5]);

			// ACT
			bool actualResult = _instance.IsExtractedTextMissing();

			// ASSERT
			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(0)]
		[TestCase(5)]
		[TestCase(9)]
		public void ItShouldGetFieldArtifactIdFromFieldName(int fieldIndex)
        {
            kCura.WinEDDS.ViewFieldInfo[] fields = _fieldFactory.GetAllDocumentFields().ToArray();

			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns((string fieldName) => fields.ToList().FindIndex(x => x.AvfColumnName == fieldName));
			_fieldService.Setup(x => x.GetColumns()).Returns(fields);

			// ACT
			int actualFieldArtifactId = _instance.GetFieldArtifactId(fields[fieldIndex].AvfColumnName);

			// ASSERT
			Assert.That(actualFieldArtifactId, Is.EqualTo(fields[fieldIndex].FieldArtifactId));
		}

		[Test]
		public void ItShouldGetFieldArtifactIdForMissingExtractedTextField()
		{
			_exportSettings.LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText;

			_fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(1);

			_fieldService.Setup(x => x.GetColumns()).Returns(new kCura.WinEDDS.ViewFieldInfo[1]);

			// ACT
			int actualFieldArtifactId = _instance.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);

			// ASSERT
			Assert.That(actualFieldArtifactId, Is.EqualTo(-1));
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(100)]
		public void ItShouldReturnTextPrecedenceIsSet(int textFieldsArrayLength)
		{
			_exportSettings.SelectedTextFields = new kCura.WinEDDS.ViewFieldInfo[textFieldsArrayLength];
			_exportSettings.SelectedTextFields[textFieldsArrayLength - 1] = _fieldFactory.GetArtifactIdField();

			// ACT
			bool isTextPrecedenceSet = _instance.IsTextPrecedenceSet();

			// ASSERT
			Assert.That(isTextPrecedenceSet, Is.True);
		}

		[Test]
		public void ItShouldReturnTextPrecedenceIsNotSetWhenTextFieldsArrayIsMissing()
		{
			_exportSettings.SelectedTextFields = null;

			// ACT
			bool isTextPrecedenceSet = _instance.IsTextPrecedenceSet();

			// ASSERT
			Assert.That(isTextPrecedenceSet, Is.False);
		}

		[Test]
		public void ItShouldReturnTextPrecedenceIsNotSetWhenTextFieldsArrayIsEmpty()
		{
			_exportSettings.SelectedTextFields = new kCura.WinEDDS.ViewFieldInfo[0];

			// ACT
			bool isTextPrecedenceSet = _instance.IsTextPrecedenceSet();

			// ASSERT
			Assert.That(isTextPrecedenceSet, Is.False);
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(100)]
		public void ItShouldReturnTextPrecedenceIsNotSetWhenAllTextFieldsAreMissing(int textFieldsArrayLength)
		{
			_exportSettings.SelectedTextFields = new kCura.WinEDDS.ViewFieldInfo[textFieldsArrayLength];

			// ACT
			bool isTextPrecedenceSet = _instance.IsTextPrecedenceSet();

			// ASSERT
			Assert.That(isTextPrecedenceSet, Is.False);
		}

		[Test]
		public void ItShouldReturnTextPrecedenceField()
		{
			const int field1ArtifactId = 795781;
			const int field2ArtifactId = 891858;

			FieldStub field1 = new FieldStub(_fieldFactory.GetArtifactIdField());
			FieldStub field2 = new FieldStub(_fieldFactory.GetArtifactIdField());

			field1.SetFieldArtifactId(field1ArtifactId);
			field2.SetFieldArtifactId(field2ArtifactId);

			_exportSettings.SelectedTextFields = new kCura.WinEDDS.ViewFieldInfo[] { field1, field2 };

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { field2ArtifactId } };

			_fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(0);

            // ACT
            kCura.WinEDDS.ViewFieldInfo textPrecedenceField = _instance.GetTextPrecedenceField(artifact);

			// ASSERT
			Assert.That(textPrecedenceField, Is.EqualTo(field2));
		}

		[Test]
		public void ItShouldReturnLongTextFileUnicodeEncoding()
		{
			FieldStub field = new FieldStub(_fieldFactory.GetArtifactIdField());
			field.SetIsUnicodeEnabled(true);

			// ACT
			Encoding fieldEncoding = _instance.GetLongTextFieldFileEncoding(field);

			// ASSERT
			Assert.That(fieldEncoding, Is.EqualTo(Encoding.Unicode));
		}

		[Test]
		public void ItShouldReturnLongTextFileDefaultEncoding()
		{
			FieldStub field = new FieldStub(_fieldFactory.GetArtifactIdField());
			field.SetIsUnicodeEnabled(false);

			// ACT
			Encoding fieldEncoding = _instance.GetLongTextFieldFileEncoding(field);

			// ASSERT
			Assert.That(fieldEncoding, Is.EqualTo(Encoding.Default));
		}
	}
}
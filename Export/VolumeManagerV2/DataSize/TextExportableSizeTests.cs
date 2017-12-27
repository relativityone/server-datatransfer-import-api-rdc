using System.Collections.Generic;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.DataSize
{
	[TestFixture]
	public class TextExportableSizeTests
	{
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;

		private TextExportableSize _instance;

		private Mock<IFieldService> _fieldService;

		private const long _TEXT_FILE_COUNT = 660488;
		private const long _TEXT_FILE_SIZE = 444442;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			_volumePredictions = new VolumePredictions
			{
				TextFileCount = _TEXT_FILE_COUNT,
				TextFilesSize = _TEXT_FILE_SIZE
			};

			_fieldService = new Mock<IFieldService>();

			_instance = new TextExportableSize(_exportSettings, new LongTextHelper(_exportSettings, _fieldService.Object, null), _fieldService.Object);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingText()
		{
			_exportSettings.ExportFullText = false;
			_exportSettings.ExportFullTextAsFile = true;

			//ACT
			_instance.CalculateTextSize(_volumePredictions, null);

			//ASSERT
			Assert.That(_volumePredictions.TextFileCount, Is.Zero);
			Assert.That(_volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingTextAsFile()
		{
			_exportSettings.ExportFullText = true;
			_exportSettings.ExportFullTextAsFile = false;

			//ACT
			_instance.CalculateTextSize(_volumePredictions, null);

			//ASSERT
			Assert.That(_volumePredictions.TextFileCount, Is.Zero);
			Assert.That(_volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNoTextFieldsSelected()
		{
			_exportSettings.ExportFullText = true;
			_exportSettings.ExportFullTextAsFile = true;
			_exportSettings.SelectedTextFields = null;

			//ACT
			_instance.CalculateTextSize(_volumePredictions, null);

			//ASSERT
			Assert.That(_volumePredictions.TextFileCount, Is.Zero);
			Assert.That(_volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		[TestCaseSource(nameof(NonTextFieldDataSet))]
		public void ItShouldNotCountFieldsOtherThanText(ViewFieldInfo field)
		{
			SetExportingTextAsFiles();

			ViewFieldInfo[] fields = {field};
			_fieldService.Setup(x => x.GetColumns()).Returns(fields);

			const int textFileCount = 3;
			const int textFileSize = 978153;

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = textFileCount,
				TextFilesSize = textFileSize
			};

			//ACT
			_instance.CalculateTextSize(predictions, new ObjectExportInfo());

			//ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(textFileCount));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(textFileSize));
		}

		[Test]
		[TestCase(FieldTypeHelper.FieldType.Text)]
		[TestCase(FieldTypeHelper.FieldType.OffTableText)]
		public void ItShouldUpdatePredictionsForTextField(FieldTypeHelper.FieldType fieldType)
		{
			const string fieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			SetUpMocksForField(fieldType, Encoding.UTF8);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {fieldTextValue}
			};

			//ACT
			_instance.CalculateTextSize(predictions, artifact);

			//ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = _exportSettings.TextFileEncoding.GetByteCount(fieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ItShouldUpdatePredictionsForTextField(Encoding encoding)
		{
			const string fieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			SetUpMocksForField(FieldTypeHelper.FieldType.Text, encoding);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {fieldTextValue}
			};

			//ACT
			_instance.CalculateTextSize(predictions, artifact);

			//ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = encoding.GetByteCount(fieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		public void ItShouldHandleTextFieldWithShibboleth_Unicode()
		{
			const long textSize = 280402;

			SetUpMocksForField(FieldTypeHelper.FieldType.Text, Encoding.Unicode);

			_fieldService.Setup(x => x.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN,
					textSize
				}
			};

			//ACT
			_instance.CalculateTextSize(predictions, artifact);

			//ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(textSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ItShouldHandleTextFieldWithShibboleth(Encoding encoding)
		{
			const long textSize = 893212;

			SetUpMocksForField(FieldTypeHelper.FieldType.Text, encoding);

			_fieldService.Setup(x => x.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN,
					textSize
				}
			};

			//ACT
			_instance.CalculateTextSize(predictions, artifact);

			//ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			long expectedFileSize = EncodingFileSize.CalculateLongTextFileSize(textSize, encoding);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedFileSize));
		}

		#region Helpers

		private static IEnumerable<Encoding> GetSampleEncodings()
		{
			return new[]
			{
				Encoding.ASCII,
				Encoding.UTF8,
				Encoding.BigEndianUnicode,
				Encoding.Default,
				Encoding.GetEncoding(1250)
			};
		}

		private void SetUpMocksForField(FieldTypeHelper.FieldType fieldType, Encoding encoding)
		{
			SetExportingTextAsFiles();
			_exportSettings.TextFileEncoding = encoding;

			FieldStub fieldStub = GetTextFieldStub();
			fieldStub.SetType(fieldType);

			CoalescedTextViewField field = new CoalescedTextViewField(fieldStub, true);

			_fieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[] {field});
			_fieldService.Setup(x => x.GetOrdinalIndex(field.AvfColumnName)).Returns(0);
		}

		private void SetExportingTextAsFiles()
		{
			_exportSettings.ExportFullText = true;
			_exportSettings.ExportFullTextAsFile = true;
			_exportSettings.SelectedTextFields = new ViewFieldInfo[1];
		}

		private static IEnumerable<ViewFieldInfo> NonTextFieldDataSet()
		{
			var fieldFactory = new QueryFieldFactory();
			ViewFieldInfo[] fields = fieldFactory.GetAllDocumentFields();
			foreach (ViewFieldInfo field in fields)
			{
				if (field.FieldType == FieldTypeHelper.FieldType.Text || field.FieldType == FieldTypeHelper.FieldType.OffTableText)
				{
					continue;
				}

				yield return field;
			}
		}

		private static FieldStub GetTextFieldStub()
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			ViewFieldInfo field = fieldFactory.GetExtractedTextField();
			return new FieldStub(field);
		}

		#endregion
	}
}
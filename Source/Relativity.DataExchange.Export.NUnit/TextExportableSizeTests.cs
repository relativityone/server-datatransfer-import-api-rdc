// -----------------------------------------------------------------------------------------------------
// <copyright file="TextExportableSizeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.DataSize;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class TextExportableSizeTests
	{
		private const long _TEXT_FILE_COUNT = 660488;
		private const long _TEXT_FILE_SIZE = 444442;
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;
		private TextExportableSize _instance;
		private Mock<IFieldService> _fieldService;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			this._volumePredictions = new VolumePredictions
			{
				TextFileCount = _TEXT_FILE_COUNT,
				TextFilesSize = _TEXT_FILE_SIZE
			};

			this._fieldService = new Mock<IFieldService>();

			this._instance = new TextExportableSize(this._exportSettings, new LongTextHelper(this._exportSettings, this._fieldService.Object, null), this._fieldService.Object);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingText()
		{
			this._exportSettings.ExportFullText = false;
			this._exportSettings.ExportFullTextAsFile = true;

			// ACT
			this._instance.CalculateTextSize(this._volumePredictions, null);

			// ASSERT
			Assert.That(this._volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this._volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingTextAsFile()
		{
			this._exportSettings.ExportFullText = true;
			this._exportSettings.ExportFullTextAsFile = false;

			// ACT
			this._instance.CalculateTextSize(this._volumePredictions, null);

			// ASSERT
			Assert.That(this._volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this._volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNoTextFieldsSelected()
		{
			this._exportSettings.ExportFullText = true;
			this._exportSettings.ExportFullTextAsFile = true;
			this._exportSettings.SelectedTextFields = null;

			// ACT
			this._instance.CalculateTextSize(this._volumePredictions, null);

			// ASSERT
			Assert.That(this._volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this._volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		[TestCaseSource(nameof(NonTextFieldDataSet))]
		public void ItShouldNotCountFieldsOtherThanText(kCura.WinEDDS.ViewFieldInfo field)
		{
			this.SetExportingTextAsFiles();

			kCura.WinEDDS.ViewFieldInfo[] fields = { field };
			this._fieldService.Setup(x => x.GetColumns()).Returns(fields);

			const int textFileCount = 3;
			const int textFileSize = 978153;

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = textFileCount,
				TextFilesSize = textFileSize
			};

			// ACT
			this._instance.CalculateTextSize(predictions, new ObjectExportInfo());

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(textFileCount));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(textFileSize));
		}

		[Test]
		[TestCase(FieldType.Text)]
		[TestCase(FieldType.OffTableText)]
		public void ItShouldUpdatePredictionsForTextField(FieldType fieldType)
		{
			const string fieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			this.SetUpMocksForField(fieldType, Encoding.UTF8);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { fieldTextValue } };

			// ACT
			this._instance.CalculateTextSize(predictions, artifact);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = this._exportSettings.TextFileEncoding.GetByteCount(fieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ItShouldUpdatePredictionsForTextField(Encoding encoding)
		{
			const string fieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			this.SetUpMocksForField(FieldType.Text, encoding);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { fieldTextValue } };

			// ACT
			this._instance.CalculateTextSize(predictions, artifact);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = encoding.GetByteCount(fieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		public void ItShouldHandleTextFieldWithShibboleth_Unicode()
		{
			const long textSize = 280402;

			this.SetUpMocksForField(FieldType.Text, Encoding.Unicode);

			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);
			this._fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(true);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
                    ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN,
					textSize
				}
			};

			// ACT
			this._instance.CalculateTextSize(predictions, artifact);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(textSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ItShouldHandleTextFieldWithShibboleth(Encoding encoding)
		{
			const long textSize = 893212;

			this.SetUpMocksForField(FieldType.Text, encoding);

			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);
			this._fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(true);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN,
					textSize
				}
			};

			// ACT
			this._instance.CalculateTextSize(predictions, artifact);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			long expectedFileSize = EncodingFileSize.CalculateLongTextFileSize(textSize, encoding);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedFileSize));
		}

		[Test]
		public void ItShouldKeepBackwardCompatibility()
		{
			const long extractedTextSizeNaive = 2097152;

			this.SetUpMocksForField(FieldType.Text, Encoding.Unicode);

			this._fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(false);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN
				}
			};

			// ACT & ASSERT
			Assert.DoesNotThrow(() => this._instance.CalculateTextSize(predictions, artifact));

			Assert.That(predictions.TextFilesSize, Is.EqualTo(extractedTextSizeNaive));
		}

		private static IEnumerable<kCura.WinEDDS.ViewFieldInfo> NonTextFieldDataSet()
		{
			var fieldFactory = new QueryFieldFactory();
			IEnumerable<kCura.WinEDDS.ViewFieldInfo> fields = fieldFactory.GetAllDocumentFields();
			foreach (kCura.WinEDDS.ViewFieldInfo field in fields)
			{
				if (field.FieldType == FieldType.Text
				    || field.FieldType == FieldType.OffTableText)
				{
					continue;
				}

				yield return field;
			}
		}

		private static FieldStub GetTextFieldStub()
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			kCura.WinEDDS.ViewFieldInfo field = fieldFactory.GetExtractedTextField();
			return new FieldStub(field);
		}

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

		private void SetUpMocksForField(FieldType fieldType, Encoding encoding)
		{
			this.SetExportingTextAsFiles();
			this._exportSettings.TextFileEncoding = encoding;

            FieldStub fieldStub = GetTextFieldStub();
			fieldStub.SetType(fieldType);

			CoalescedTextViewField field = new CoalescedTextViewField(fieldStub, true);

			this._fieldService.Setup(x => x.GetColumns()).Returns(new kCura.WinEDDS.ViewFieldInfo[] { field });
			this._fieldService.Setup(x => x.GetOrdinalIndex(field.AvfColumnName)).Returns(0);
		}

		private void SetExportingTextAsFiles()
		{
			this._exportSettings.ExportFullText = true;
			this._exportSettings.ExportFullTextAsFile = true;
			this._exportSettings.SelectedTextFields = new kCura.WinEDDS.ViewFieldInfo[1];
		}
	}
}
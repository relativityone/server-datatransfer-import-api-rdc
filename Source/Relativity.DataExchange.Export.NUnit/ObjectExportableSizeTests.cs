// ----------------------------------------------------------------------------
// <copyright file="ObjectExportableSizeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
    using System;
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
    using ViewFieldInfo = kCura.WinEDDS.ViewFieldInfo;

	[TestFixture]
	public class ObjectExportableSizeTests
	{
		private const long NativeFileSize = 647109;
		private const long NativeFileCount = 771933;
		private const long PdfFileSize = 345312;
		private const long PdfFileCount = 429605;
		private const long ImageFileSize = 505718;
		private const long ImageFileCount = 959625;
		private const long TextFileCount = 660488;
		private const long TextFileSize = 444442;

		private ExportFile exportSettings;
		private VolumePredictions volumePredictions;
		private Mock<IFieldService> fieldService;
		private ObjectExportableSize instance;

		[SetUp]
		public void SetUp()
		{
			this.exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			this.volumePredictions = new VolumePredictions
			{
				NativeFileCount = NativeFileCount,
				NativeFilesSize = NativeFileSize,
				PdfFileCount = PdfFileCount,
				PdfFileSize = PdfFileSize,
				ImageFileCount = ImageFileCount,
				ImageFilesSize = ImageFileSize,
				TextFileCount = TextFileCount,
				TextFilesSize = TextFileSize
			};

			this.fieldService = new Mock<IFieldService>();
			this.instance = new ObjectExportableSize(this.exportSettings, new LongTextHelper(this.exportSettings, this.fieldService.Object, null), this.fieldService.Object);
		}

		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		public void ShouldResetSizeAndCountWhenNotExportingNatives(bool exportNative, bool copyNative)
		{
			// ARRANGE
			this.exportSettings.ExportNative = exportNative;
			this.exportSettings.VolumeInfo.CopyNativeFilesFromRepository = copyNative;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.NativeFileCount, Is.Zero);
			Assert.That(this.volumePredictions.NativeFilesSize, Is.Zero);
		}

		[Test]
		public void ShouldNotChangeSizeAndCountWhenExportingNative()
		{
			// ARRANGE
			this.exportSettings.ExportNative = true;
			this.exportSettings.VolumeInfo.CopyNativeFilesFromRepository = true;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.NativeFilesSize, Is.EqualTo(NativeFileSize));
			Assert.That(this.volumePredictions.NativeFileCount, Is.EqualTo(NativeFileCount));
		}

		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		public void ShouldResetSizeAndCountWhenNotExportingPdf(bool exportPdf, bool copyPdf)
		{
			// ARRANGE
			this.exportSettings.ExportPdf = exportPdf;
			this.exportSettings.VolumeInfo.CopyPdfFilesFromRepository = copyPdf;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.PdfFileSize, Is.Zero);
			Assert.That(this.volumePredictions.PdfFileCount, Is.Zero);
		}

		[Test]
		public void ShouldNotChangeSizeAndCountWhenExportingPdf()
		{
			// ARRANGE
			this.exportSettings.ExportPdf = true;
			this.exportSettings.VolumeInfo.CopyPdfFilesFromRepository = true;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.PdfFileSize, Is.EqualTo(PdfFileSize));
			Assert.That(this.volumePredictions.PdfFileCount, Is.EqualTo(PdfFileCount));
		}

		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		public void ShouldResetSizeAndCountWhenNotExportingImages(bool exportImages, bool copyImages)
		{
			// ARRANGE
			this.exportSettings.ExportImages = exportImages;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = copyImages;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFilesSize, Is.Zero);
			Assert.That(this.volumePredictions.ImageFileCount, Is.Zero);
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.SinglePage)]
		public void ShouldNotChangeSizeWhenExportingTiffImages(ExportFile.ImageType imageType)
		{
			// ARRANGE
			this.exportSettings.ExportImages = true;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this.exportSettings.TypeOfImage = imageType;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFilesSize, Is.EqualTo(ImageFileSize));
		}

		[Test]
		public void ShouldIncreaseSizeWhenExportingImagesAsPdf()
		{
			// ARRANGE
			const double PdfMergeSizeErrorThreshold = 1.03;

			this.exportSettings.ExportImages = true;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this.exportSettings.TypeOfImage = ExportFile.ImageType.Pdf;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFilesSize, Is.EqualTo(Math.Ceiling(ImageFileSize * PdfMergeSizeErrorThreshold)));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ShouldChangeCountWhenMergingImages(ExportFile.ImageType imageType)
		{
			// ARRANGE
			this.exportSettings.ExportImages = true;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this.exportSettings.TypeOfImage = imageType;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFileCount, Is.EqualTo(1));
		}

		[Test]
		public void ShouldNotChangeCountWhenNotMergingImages()
		{
			// ARRANGE
			this.exportSettings.ExportImages = true;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this.exportSettings.TypeOfImage = ExportFile.ImageType.SinglePage;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFileCount, Is.EqualTo(ImageFileCount));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ShouldNotChangeCountWhenNoImagesToMerge(ExportFile.ImageType imageType)
		{
			// ARRANGE
			this.exportSettings.ExportImages = true;
			this.exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this.exportSettings.TypeOfImage = imageType;
			this.volumePredictions.ImageFileCount = 0;

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.ImageFileCount, Is.Zero);
		}

		[Test]
		public void ShouldResetSizeAndCountWhenNotExportingText()
		{
			// ARRANGE
			this.exportSettings.ExportFullText = false;
			this.exportSettings.ExportFullTextAsFile = true;

			// ACT
			this.instance.FinalizeSizeCalculations(null, this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this.volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ShouldResetSizeAndCountWhenNotExportingTextAsFile()
		{
			// ARRANGE
			this.exportSettings.ExportFullText = true;
			this.exportSettings.ExportFullTextAsFile = false;

			// ACT
			this.instance.FinalizeSizeCalculations(null, this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this.volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ShouldResetSizeAndCountWhenNoTextFieldsSelected()
		{
			// ARRANGE
			this.exportSettings.ExportFullText = true;
			this.exportSettings.ExportFullTextAsFile = true;
			this.exportSettings.SelectedTextFields = null;

			// ACT
			this.instance.FinalizeSizeCalculations(null, this.volumePredictions);

			// ASSERT
			Assert.That(this.volumePredictions.TextFileCount, Is.Zero);
			Assert.That(this.volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		[TestCaseSource(nameof(NonTextFieldDataSet))]
		public void ShouldNotCountFieldsOtherThanText(ViewFieldInfo field)
		{
			// ARRANGE
			this.SetExportingTextAsFiles();

			ViewFieldInfo[] fields = { field };
			this.fieldService.Setup(x => x.GetColumns()).Returns(fields);

			const int TextCount = 3;
			const int TextSize = 978153;

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = TextCount,
				TextFilesSize = TextSize
			};

			// ACT
			this.instance.FinalizeSizeCalculations(new ObjectExportInfo(), predictions);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(TextCount));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(TextSize));
		}

		[Test]
		[TestCase(FieldType.Text)]
		[TestCase(FieldType.OffTableText)]
		public void ShouldUpdatePredictionsForTextField(FieldType fieldType)
		{
			// ARRANGE
			const string FieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			this.SetUpMocksForField(fieldType, Encoding.UTF8);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { FieldTextValue } };

			// ACT
			this.instance.FinalizeSizeCalculations(artifact, predictions);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = this.exportSettings.TextFileEncoding.GetByteCount(FieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ShouldUpdatePredictionsForTextField(Encoding encoding)
		{
			// ARRANGE
			const string FieldTextValue = "Lorem ipsum dolor sit amet enim. Etiam ullamcorper.";

			this.SetUpMocksForField(FieldType.Text, encoding);

			VolumePredictions predictions = new VolumePredictions
			{
				TextFileCount = 0,
				TextFilesSize = 0
			};

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { FieldTextValue } };

			// ACT
			this.instance.FinalizeSizeCalculations(artifact, predictions);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			int expectedSize = encoding.GetByteCount(FieldTextValue);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedSize));
		}

		[Test]
		public void ShouldHandleTextFieldWithShibboleth_Unicode()
		{
			// ARRANGE
			const long TextSize = 280402;

			this.SetUpMocksForField(FieldType.Text, Encoding.Unicode);

			this.fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);
			this.fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(true);

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
					TextSize
				}
			};

			// ACT
			this.instance.FinalizeSizeCalculations(artifact, predictions);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(TextSize));
		}

		[Test]
		[TestCaseSource(nameof(GetSampleEncodings))]
		public void ShouldHandleTextFieldWithShibboleth(Encoding encoding)
		{
			// ARRANGE
			const long TextSize = 893212;

			this.SetUpMocksForField(FieldType.Text, encoding);

			this.fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(1);
			this.fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(true);

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
					TextSize
				}
			};

			// ACT
			this.instance.FinalizeSizeCalculations(artifact, predictions);

			// ASSERT
			Assert.That(predictions.TextFileCount, Is.EqualTo(1));
			long expectedFileSize = EncodingFileSize.CalculateLongTextFileSize(TextSize, encoding);
			Assert.That(predictions.TextFilesSize, Is.EqualTo(expectedFileSize));
		}

		[Test]
		public void ShouldKeepBackwardCompatibility()
		{
			// ARRANGE
			const long ExtractedTextSizeNaive = 2097152;

			this.SetUpMocksForField(FieldType.Text, Encoding.Unicode);

			this.fieldService.Setup(x => x.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE)).Returns(false);

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
			Assert.DoesNotThrow(() => this.instance.FinalizeSizeCalculations(artifact, predictions));
			Assert.That(predictions.TextFilesSize, Is.EqualTo(ExtractedTextSizeNaive));
		}

		private static IEnumerable<ViewFieldInfo> NonTextFieldDataSet()
		{
			var fieldFactory = new QueryFieldFactory();
			IEnumerable<ViewFieldInfo> fields = fieldFactory.GetAllDocumentFields();
			foreach (ViewFieldInfo field in fields)
			{
				if (field.FieldType == FieldType.Text
					|| field.FieldType == FieldType.OffTableText)
				{
					continue;
				}

				yield return field;
			}
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

		private static FieldStub GetTextFieldStub()
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			ViewFieldInfo field = fieldFactory.GetExtractedTextField();
			return new FieldStub(field);
		}

		private void SetUpMocksForField(FieldType fieldType, Encoding encoding)
		{
			this.SetExportingTextAsFiles();
			this.exportSettings.TextFileEncoding = encoding;

			FieldStub fieldStub = GetTextFieldStub();
			fieldStub.SetType(fieldType);

			CoalescedTextViewField field = new CoalescedTextViewField(fieldStub, true);

			this.fieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[] { field });
			this.fieldService.Setup(x => x.GetOrdinalIndex(field.AvfColumnName)).Returns(0);
		}

		private void SetExportingTextAsFiles()
		{
			this.exportSettings.ExportFullText = true;
			this.exportSettings.ExportFullTextAsFile = true;
			this.exportSettings.SelectedTextFields = new ViewFieldInfo[1];
		}
	}
}
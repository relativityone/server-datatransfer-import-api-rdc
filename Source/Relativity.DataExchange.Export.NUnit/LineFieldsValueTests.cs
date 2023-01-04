// -----------------------------------------------------------------------------------------------------
// <copyright file="LineFieldsValueTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LineFieldsValueTests
	{
		private const char _QUOTE_DELIMITER = 'Q';
		private const char _RECORD_DELIMITER = 'R';
		private LineFieldsValue _instance;
		private QueryFieldFactory _queryFieldFactory;
		private Mock<IFieldService> _fieldLookupService;
		private Mock<ILongTextHandler> _longTextHandler;

		[SetUp]
		public void SetUp()
		{
			this._queryFieldFactory = new QueryFieldFactory();

			ExportFile exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				RecordDelimiter = _RECORD_DELIMITER
			};

			this._fieldLookupService = new Mock<IFieldService>();
			this._longTextHandler = new Mock<ILongTextHandler>();

			this._instance = new LineFieldsValue(
				this._fieldLookupService.Object,
				this._longTextHandler.Object,
				new LongTextHelper(null, null, null),
				new NonTextFieldHandler(this._fieldLookupService.Object, new DelimitedCellFormatter(exportSettings), exportSettings, new TestNullLogger()),
				exportSettings,
				new TestNullLogger());
		}

		[Test]
		public void ItShouldDistinguishLongTextFields()
		{
			kCura.WinEDDS.ViewFieldInfo artifactIdField = this._queryFieldFactory.GetArtifactIdField();
            kCura.WinEDDS.ViewFieldInfo extractedTextField = this._queryFieldFactory.GetExtractedTextField();
            ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { string.Empty } };

            this._fieldLookupService.Setup(x => x.GetColumns()).Returns(new[] { artifactIdField, extractedTextField });
			this._fieldLookupService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.AddFieldsValue(lineEntry, artifact);

			// ASSERT
			this._longTextHandler.Verify(x => x.HandleLongText(artifact, extractedTextField, lineEntry), Times.Once);
			this._longTextHandler.Verify(x => x.HandleLongText(artifact, artifactIdField, lineEntry), Times.Never);
		}

		[Test]
		public void ItShouldCreateLineForFields()
		{
			ObjectExportInfo artifact = new ObjectExportInfo();
			List<kCura.WinEDDS.ViewFieldInfo> fields = this.PrepareDataSet(artifact);

			DeferredEntry loadFileEntry = new DeferredEntry();

			IEnumerable<string> fieldsEntries = fields.Select(FormatFieldValue);
			string expectedResult = string.Join(_RECORD_DELIMITER.ToString(), fieldsEntries);

			// ACT
			this._instance.AddFieldsValue(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedResult));
		}

		private static string FormatFieldValue(kCura.WinEDDS.ViewFieldInfo x)
		{
			return $"{_QUOTE_DELIMITER}{x.AvfColumnName}{_QUOTE_DELIMITER}";
		}

		private List<kCura.WinEDDS.ViewFieldInfo> PrepareDataSet(ObjectExportInfo artifact)
        {
            kCura.WinEDDS.ViewFieldInfo[] allDocumentFields =
                this._queryFieldFactory.GetAllDocumentFields().ToArray();
			List<kCura.WinEDDS.ViewFieldInfo> fields = allDocumentFields
				.Where(x => x.FieldType != FieldType.Text && x.FieldType != FieldType.OffTableText).ToList();

			List<string> fieldValues = new List<string>();
			foreach (var field in fields)
			{
				fieldValues.Add(field.AvfColumnName);
			}

			artifact.Metadata = fieldValues.Cast<object>().ToArray();

			this._fieldLookupService.Setup(x => x.GetColumns()).Returns(allDocumentFields.ToArray);
			this._fieldLookupService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns((string columnName) => fields.FindIndex(x => x.AvfColumnName == columnName));

			this._longTextHandler.Setup(x => x.HandleLongText(It.IsAny<ObjectExportInfo>(), It.IsAny<kCura.WinEDDS.ViewFieldInfo>(), It.IsAny<DeferredEntry>()))
				.Callback((ObjectExportInfo a, kCura.WinEDDS.ViewFieldInfo f, DeferredEntry l) => l.AddStringEntry(FormatFieldValue(f)));

			return allDocumentFields.ToList();
		}
	}
}
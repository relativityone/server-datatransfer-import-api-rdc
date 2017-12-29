using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Natives
{
	[TestFixture]
	public class LineFieldsValueTests
	{
		private LineFieldsValue _instance;
		private QueryFieldFactory _queryFieldFactory;

		private Mock<IFieldService> _fieldLookupService;
		private Mock<ILongTextHandler> _longTextHandler;

		private const char _QUOTE_DELIMITER = 'Q';
		private const char _RECORD_DELIMITER = 'R';

		[SetUp]
		public void SetUp()
		{
			_queryFieldFactory = new QueryFieldFactory();

			ExportFile exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = _QUOTE_DELIMITER,
				RecordDelimiter = _RECORD_DELIMITER
			};

			_fieldLookupService = new Mock<IFieldService>();
			_longTextHandler = new Mock<ILongTextHandler>();

			_instance = new LineFieldsValue(_fieldLookupService.Object, _longTextHandler.Object, new LongTextHelper(null, null, null),
				new NonTextFieldHandler(_fieldLookupService.Object, new DelimitedCellFormatter(exportSettings), exportSettings, new NullLogger()), exportSettings, new NullLogger());
		}

		[Test]
		public void ItShouldDistinguishLongTextFields()
		{
			ViewFieldInfo artifactIdField = _queryFieldFactory.GetArtifactIdField();
			ViewFieldInfo extractedTextField = _queryFieldFactory.GetExtractedTextField();


			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {""}
			};

			_fieldLookupService.Setup(x => x.GetColumns()).Returns(new[] {artifactIdField, extractedTextField});
			_fieldLookupService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			DeferredEntry lineEntry = new DeferredEntry();

			//ACT
			_instance.AddFieldsValue(lineEntry, artifact);

			//ASSERT
			_longTextHandler.Verify(x => x.HandleLongText(artifact, extractedTextField, lineEntry), Times.Once);
			_longTextHandler.Verify(x => x.HandleLongText(artifact, artifactIdField, lineEntry), Times.Never);
		}

		[Test]
		public void ItShouldCreateLineForFields()
		{
			ObjectExportInfo artifact = new ObjectExportInfo();
			List<ViewFieldInfo> fields = PrepareDataSet(artifact);

			DeferredEntry loadFileEntry = new DeferredEntry();

			IEnumerable<string> fieldsEntries = fields.Select(FormatFieldValue);
			string expectedResult = string.Join(_RECORD_DELIMITER.ToString(), fieldsEntries);

			//ACT
			_instance.AddFieldsValue(loadFileEntry, artifact);

			//ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(expectedResult));
		}

		private static string FormatFieldValue(ViewFieldInfo x)
		{
			return $"{_QUOTE_DELIMITER}{x.AvfColumnName}{_QUOTE_DELIMITER}";
		}

		private List<ViewFieldInfo> PrepareDataSet(ObjectExportInfo artifact)
		{
			ViewFieldInfo[] allDocumentFields = _queryFieldFactory.GetAllDocumentFields();

			List<ViewFieldInfo> fields = allDocumentFields
				.Where(x => x.FieldType != FieldTypeHelper.FieldType.Text && x.FieldType != FieldTypeHelper.FieldType.OffTableText).ToList();

			List<string> fieldValues = new List<string>();
			foreach (var field in fields)
			{
				fieldValues.Add(field.AvfColumnName);
			}

			artifact.Metadata = fieldValues.Cast<object>().ToArray();

			_fieldLookupService.Setup(x => x.GetColumns()).Returns(allDocumentFields);
			_fieldLookupService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns((string columnName) => fields.FindIndex(x => x.AvfColumnName == columnName));

			_longTextHandler.Setup(x => x.HandleLongText(It.IsAny<ObjectExportInfo>(), It.IsAny<ViewFieldInfo>(), It.IsAny<DeferredEntry>()))
				.Callback((ObjectExportInfo a, ViewFieldInfo f, DeferredEntry l) => l.AddStringEntry(FormatFieldValue(f)));

			return allDocumentFields.ToList();
		}
	}
}
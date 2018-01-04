using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;
using Constants = Relativity.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class LongTextFromFieldBuilderTests
	{
		private LongTextFromFieldBuilder _instance;

		private Mock<IFieldService> _fieldService;
		private QueryFieldFactory _queryFieldFactory;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1);
			_queryFieldFactory = new QueryFieldFactory();

			_fieldService = new Mock<IFieldService>();

			_instance = new LongTextFromFieldBuilder(_fieldService.Object, new LongTextHelper(exportSettings, _fieldService.Object, new LongTextRepository(null, new NullLogger())),
				new NullLogger());
		}

		[Test]
		public void ItShouldTakeOnlyLongTextFields()
		{
			ViewFieldInfo[] fields = _queryFieldFactory.GetAllDocumentFields();
			ViewFieldInfo longTextField = fields.First(x => x.FieldType == FieldTypeHelper.FieldType.Text);
			_fieldService.Setup(x => x.GetColumns()).Returns(fields);
			_fieldService.Setup(x => x.GetOrdinalIndex(longTextField.AvfColumnName)).Returns(0);

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {""}
			};

			//ACT
			IList<LongText> result = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
		}

		[Test]
		public void ItShouldTakeAllLongTextFields()
		{
			FieldStub longTextField1 = new FieldStub(_queryFieldFactory.GetExtractedTextField());
			FieldStub longTextField2 = new FieldStub(_queryFieldFactory.GetExtractedTextField());
			longTextField1.SetFieldArtifactId(111);
			longTextField2.SetFieldArtifactId(222);
			_fieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[]{longTextField1, longTextField2});
			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {""}
			};

			//ACT
			IList<LongText> result = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Any(x => x.FieldArtifactId == longTextField1.FieldArtifactId));
			Assert.That(result.Any(x => x.FieldArtifactId == longTextField2.FieldArtifactId));
		}

		[Test]
		public void ItShouldHandleNotTooLongText()
		{
			ViewFieldInfo longTextField = _queryFieldFactory.GetExtractedTextField();
			_fieldService.Setup(x => x.GetColumns()).Returns(new[] { longTextField });
			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string notTooLongText = "not too long text";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { notTooLongText }
			};

			//ACT
			IList<LongText> result = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
			Assert.That(result[0].ExportRequest, Is.Null);
			Assert.That(result[0].GetLongText().ReadToEnd(), Is.EqualTo(notTooLongText));
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			ViewFieldInfo longTextField = _queryFieldFactory.GetExtractedTextField();
			_fieldService.Setup(x => x.GetColumns()).Returns(new[] { longTextField });
			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string tooLongText = Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { tooLongText }
			};

			//ACT
			IList<LongText> result = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
			Assert.That(result[0].ExportRequest, Is.Not.Null);
			Assert.That(result[0].Location, Is.Not.Null);
		}

		[Test]
		public void ItShouldSkipTextPrecedenceField()
		{
			CoalescedTextViewField longTextField = new CoalescedTextViewField(_queryFieldFactory.GetExtractedTextField(), true);

			_fieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[] { longTextField });
			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string tooLongText = Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { tooLongText }
			};

			//ACT
			IList<LongText> result = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(result, Is.Empty);
		}
	}
}
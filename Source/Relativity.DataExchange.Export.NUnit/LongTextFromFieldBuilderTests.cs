// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextFromFieldBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

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
			this._queryFieldFactory = new QueryFieldFactory();

			this._fieldService = new Mock<IFieldService>();

			this._instance = new LongTextFromFieldBuilder(
				this._fieldService.Object,
				new LongTextHelper(exportSettings, this._fieldService.Object, new LongTextRepository(null, new TestNullLogger())),
				new TestNullLogger());
		}

		[Test]
		public void ItShouldTakeOnlyLongTextFields()
        {
            kCura.WinEDDS.ViewFieldInfo[] fields = this._queryFieldFactory.GetAllDocumentFields().ToArray();
            kCura.WinEDDS.ViewFieldInfo longTextField = fields.First(x => x.FieldType == FieldType.Text);
			this._fieldService.Setup(x => x.GetColumns()).Returns(fields);
			this._fieldService.Setup(x => x.GetOrdinalIndex(longTextField.AvfColumnName)).Returns(0);

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { string.Empty } };

			// ACT
			IList<LongText> result = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
		}

		[Test]
		public void ItShouldTakeAllLongTextFields()
		{
			FieldStub longTextField1 = new FieldStub(this._queryFieldFactory.GetExtractedTextField());
			FieldStub longTextField2 = new FieldStub(this._queryFieldFactory.GetExtractedTextField());
			longTextField1.SetFieldArtifactId(111);
			longTextField2.SetFieldArtifactId(222);
			this._fieldService.Setup(x => x.GetColumns())
				.Returns(new kCura.WinEDDS.ViewFieldInfo[] { longTextField1, longTextField2 });
			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { string.Empty } };

			// ACT
			IList<LongText> result = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Any(x => x.FieldArtifactId == longTextField1.FieldArtifactId));
			Assert.That(result.Any(x => x.FieldArtifactId == longTextField2.FieldArtifactId));
		}

		[Test]
		public void ItShouldHandleNotTooLongText()
		{
			kCura.WinEDDS.ViewFieldInfo longTextField = this._queryFieldFactory.GetExtractedTextField();
			this._fieldService.Setup(x => x.GetColumns()).Returns(new[] { longTextField });
			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string notTooLongText = "not too long text";

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { notTooLongText } };

			// ACT
			IList<LongText> result = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
			Assert.That(result[0].ExportRequest, Is.Null);
			Assert.That(result[0].GetLongText().ReadToEnd(), Is.EqualTo(notTooLongText));
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
            kCura.WinEDDS.ViewFieldInfo longTextField = this._queryFieldFactory.GetExtractedTextField();
            this._fieldService.Setup(x => x.GetColumns()).Returns(new[] { longTextField });
			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { tooLongText } };

			// ACT
			IList<LongText> result = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].FieldArtifactId, Is.EqualTo(longTextField.FieldArtifactId));
			Assert.That(result[0].ExportRequest, Is.Not.Null);
			Assert.That(result[0].Location, Is.Not.Null);
		}

		[Test]
		public void ItShouldSkipTextPrecedenceField()
		{
			CoalescedTextViewField longTextField = new CoalescedTextViewField(this._queryFieldFactory.GetExtractedTextField(), true);

			this._fieldService.Setup(x => x.GetColumns()).Returns(new kCura.WinEDDS.ViewFieldInfo[] { longTextField });
			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { tooLongText } };

			// ACT
			IList<LongText> result = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(result, Is.Empty);
		}
	}
}
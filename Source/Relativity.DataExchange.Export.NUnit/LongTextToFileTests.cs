// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextToFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	using ViewFieldInfo = kCura.WinEDDS.ViewFieldInfo;

    [TestFixture]
	public class LongTextToFileTests
	{
		private LongTextToFile _instance;

		private QueryFieldFactory _fieldFactory;
		private ExportFile _exportSettings;
		private LongTextRepository _longTextRepository;

		private Mock<IFieldService> _fieldService;
		private Mock<IFilePathTransformer> _filePathTransformer;

		[SetUp]
		public void SetUp()
		{
			this._fieldFactory = new QueryFieldFactory();
			this._exportSettings = new ExportFile(1);
			this._longTextRepository = new LongTextRepository(null, new NullLogger());

			this._fieldService = new Mock<IFieldService>();
			this._filePathTransformer = new Mock<IFilePathTransformer>();
			this._filePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns((string s) => s);

			this._instance = new LongTextToFile(
				this._exportSettings,
				this._filePathTransformer.Object,
				this._longTextRepository,
				new LongTextHelper(this._exportSettings, this._fieldService.Object, this._longTextRepository),
				new NullLogger());
		}

		[Test]
		public void ItShouldAddEntryWithTextFileLocation()
		{
			const string expectedResult = "file_location";

			ViewFieldInfo field = this._fieldFactory.GetArtifactIdField();
			ObjectExportInfo artifact = new ObjectExportInfo();

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				field.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, expectedResult),
				Encoding.Default);
			this._longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
			this._filePathTransformer.Verify(x => x.TransformPath(expectedResult));
		}

		[Test]
		public void ItShouldAddEntryWithTextFileLocationForHtmlFile()
		{
			const string location = "location";
			const string expectedResult = "<a href='location' target='_textwindow'>location</a>";

			this._exportSettings.LoadFileIsHtml = true;

			ViewFieldInfo field = this._fieldFactory.GetArtifactIdField();
			ObjectExportInfo artifact = new ObjectExportInfo();

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				field.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, location),
				Encoding.Default);
			this._longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
			this._filePathTransformer.Verify(x => x.TransformPath(location));
		}

		[Test]
		public void ItShouldUseTrueTextPrecedenceField()
		{
			const string expectedResult = "file_location";

			FieldStub textPrecedenceField = new FieldStub(this._fieldFactory.GetArtifactIdField());
			textPrecedenceField.SetAvfColumnName(ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME);

			ViewFieldInfo trueTextPrecedenceField = this._fieldFactory.GetArtifactIdField();

			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            Metadata = new object[] { trueTextPrecedenceField.FieldArtifactId }
				                            };

			this._exportSettings.SelectedTextFields = new[] { trueTextPrecedenceField };

			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(0);

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				trueTextPrecedenceField.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(
					artifact,
					trueTextPrecedenceField.FieldArtifactId,
					expectedResult),
				Encoding.Default);
			this._longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, textPrecedenceField, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
		}
	}
}
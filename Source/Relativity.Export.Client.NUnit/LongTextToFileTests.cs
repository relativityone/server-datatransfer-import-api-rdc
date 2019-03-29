// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextToFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Text;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;
    using kCura.WinEDDS.LoadFileEntry;

    using Moq;

	using Relativity.Export.VolumeManagerV2;
	using Relativity.Export.VolumeManagerV2.Directories;
	using Relativity.Export.VolumeManagerV2.Download;
	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Export.VolumeManagerV2.Repository;
	using Relativity.Import.Export.TestFramework;
    using Relativity.Logging;

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
			_fieldFactory = new QueryFieldFactory();
			_exportSettings = new ExportFile(1);
			_longTextRepository = new LongTextRepository(null, new NullLogger());

			_fieldService = new Mock<IFieldService>();
			_filePathTransformer = new Mock<IFilePathTransformer>();
			_filePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns((string s) => s);

			_instance = new LongTextToFile(
				_exportSettings,
				_filePathTransformer.Object,
				_longTextRepository,
				new LongTextHelper(_exportSettings, _fieldService.Object, _longTextRepository),
				new NullLogger());
		}

		[Test]
		public void ItShouldAddEntryWithTextFileLocation()
		{
			const string expectedResult = "file_location";

			ViewFieldInfo field = _fieldFactory.GetArtifactIdField();
			ObjectExportInfo artifact = new ObjectExportInfo();

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				field.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, expectedResult),
				Encoding.Default);
			_longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			_instance.HandleLongText(artifact, field, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
			_filePathTransformer.Verify(x => x.TransformPath(expectedResult));
		}

		[Test]
		public void ItShouldAddEntryWithTextFileLocationForHtmlFile()
		{
			const string location = "location";
			const string expectedResult = "<a href='location' target='_textwindow'>location</a>";

			_exportSettings.LoadFileIsHtml = true;

			ViewFieldInfo field = _fieldFactory.GetArtifactIdField();
			ObjectExportInfo artifact = new ObjectExportInfo();

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				field.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, location),
				Encoding.Default);
			_longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			_instance.HandleLongText(artifact, field, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
			_filePathTransformer.Verify(x => x.TransformPath(location));
		}

		[Test]
		public void ItShouldUseTrueTextPrecedenceField()
		{
			const string expectedResult = "file_location";

			FieldStub textPrecedenceField = new FieldStub(_fieldFactory.GetArtifactIdField());
			textPrecedenceField.SetAvfColumnName(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME);

			ViewFieldInfo trueTextPrecedenceField = _fieldFactory.GetArtifactIdField();

			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            Metadata = new object[] { trueTextPrecedenceField.FieldArtifactId }
				                            };

			_exportSettings.SelectedTextFields = new[] { trueTextPrecedenceField };

			_fieldService.Setup(x => x.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(0);

			LongText longText = LongText.CreateFromMissingValue(
				artifact.ArtifactID,
				trueTextPrecedenceField.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(
					artifact,
					trueTextPrecedenceField.FieldArtifactId,
					expectedResult),
				Encoding.Default);
			_longTextRepository.Add(longText.InList());

			DeferredEntry entry = new DeferredEntry();

			// ACT
			_instance.HandleLongText(artifact, textPrecedenceField, entry);

			// ASSERT
			Assert.That(entry.GetTextFromEntry(), Is.EqualTo(expectedResult));
		}
	}
}
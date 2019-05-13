// -----------------------------------------------------------------------------------------------------
// <copyright file="TooLongTextToLoadFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.IO;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	[TestFixture]
	public class TooLongTextToLoadFileTests
	{
		private TooLongTextToLoadFile _instance;

		private LongTextRepository _longTextRepository;
		private ExportFile _exportSettings;

		private Mock<IFieldService> _fieldService;
		private Mock<ILongTextEntryWriter> _fileWriter;

		[SetUp]
		public void SetUp()
		{
			this._longTextRepository = new LongTextRepository(null, new NullLogger());

			this._exportSettings = new ExportFile(1);

			this._fieldService = new Mock<IFieldService>();

			LongTextHelper longTextHelper = new LongTextHelper(this._exportSettings, this._fieldService.Object, this._longTextRepository);

			this._fileWriter = new Mock<ILongTextEntryWriter>();

			this._instance = new TooLongTextToLoadFile(longTextHelper, this._longTextRepository, this._fileWriter.Object, new NullLogger());
		}

		[Test]
		public void ItShouldAddEntryWithLongTextDeferredEntry()
		{
			const string expectedPath = "expected_path";
			Encoding expectedEncoding = Encoding.UTF32;

			kCura.WinEDDS.ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = 676396
			};

			LongText longText = LongText.CreateFromMissingFile(
				artifact.ArtifactID,
				field.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, expectedPath),
				Encoding.Unicode,
				expectedEncoding);
			this._longTextRepository.Add(longText.InList());

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			StreamWriter writer = new StreamWriter(new MemoryStream());
			lineEntry.Write(ref writer);

			// ASSERT
			this._fileWriter.Verify(x => x.WriteLongTextFileToDatFile(writer, expectedPath, expectedEncoding));
		}

		[Test]
		public void ItShouldAddEntryWithLongTextDeferredEntryForTextPrecedence()
		{
			const string expectedPath = "expected_path";
			Encoding expectedEncoding = Encoding.UTF32;

			CoalescedTextViewField field = new CoalescedTextViewField(new QueryFieldFactory().GetArtifactIdField(), true);

			kCura.WinEDDS.ViewFieldInfo trueTextPrecedenceField = new QueryFieldFactory().GetGenericDateField();
			this._exportSettings.SelectedTextFields = new[] { trueTextPrecedenceField };

			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            ArtifactID = 501226,
					                            Metadata = new object[] { trueTextPrecedenceField.FieldArtifactId }
				                            };

			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(0);

			LongText longText = LongText.CreateFromMissingFile(
				artifact.ArtifactID,
				trueTextPrecedenceField.FieldArtifactId,
				LongTextExportRequest.CreateRequestForLongText(
					artifact,
					trueTextPrecedenceField.FieldArtifactId,
					expectedPath),
				Encoding.Unicode,
				expectedEncoding);
			this._longTextRepository.Add(longText.InList());

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			StreamWriter writer = new StreamWriter(new MemoryStream());
			lineEntry.Write(ref writer);

			// ASSERT
			this._fileWriter.Verify(x => x.WriteLongTextFileToDatFile(writer, expectedPath, expectedEncoding));
		}
	}
}
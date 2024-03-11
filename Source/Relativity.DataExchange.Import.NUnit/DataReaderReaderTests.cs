// -----------------------------------------------------------------------------------------------------
// <copyright file="DataReaderReaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Api;
	using kCura.WinEDDS.ImportExtension;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	using FieldType = Relativity.DataExchange.Service.FieldType;

	[TestFixture(10)]
	[TestFixture(1021212)]
	public class DataReaderReaderTests
	{
		private const string IdentifierFieldName = "identifierfield";
		private const string TextFieldName = "textfield";
		private int _artifactTypeID;

		public DataReaderReaderTests(int artifactTypeID)
		{
			this._artifactTypeID = artifactTypeID;
		}

		[Test]
		public void ItShouldReadRecordsAndMapLineNumberToIdentifier()
		{
			// arrange
			var dataSource = ImportDataSourceBuilder.New()
				.AddField(IdentifierFieldName, new[] { "Record 001", "Record 002", "Record 003" })
				.AddField(TextFieldName, new[] { "One", "Two", "Three" })
				.Build();

			using (var reader = new ImportDataSourceToDataReaderAdapter<object[]>(dataSource))
			{
				reader.Read(); // DataReaderReader expects the reader to be positioned at the first record

				var identifierField = CreateField(IdentifierFieldName, artifactID: 1001, isIdentifier: true);
				var allFields = new ArtifactFieldCollection
				{
					new ArtifactField(identifierField),
					new ArtifactField(CreateField(TextFieldName, artifactID: 1002, isIdentifier: false)),
				};
				var initializationArgs = new DataReaderReaderInitializationArgs(
					allFields,
					artifactTypeID: this._artifactTypeID);
				var fieldMap = new LoadFile
				{
					ArtifactTypeID = _artifactTypeID,
					SelectedIdentifierField = identifierField,
				};

				var sut = new DataReaderReader(initializationArgs, fieldMap, reader);

				// act & assert
				Assert.That(sut.CurrentLineNumber, Is.EqualTo(0));

				Assert.That(sut.HasMoreRecords, Is.True);
				var firstArtifact = sut.ReadArtifact();
				Assert.That(firstArtifact[IdentifierFieldName].Value, Is.EqualTo("Record 001"));
				Assert.That(firstArtifact[TextFieldName].Value, Is.EqualTo("One"));
				Assert.That(
					sut.CurrentLineNumber,
					Is.EqualTo(
						1)); // DataReaderReader.CurrentLineNumber is incremented after ReadArtifact, so it is 1-based

				Assert.That(sut.HasMoreRecords, Is.True);
				var secondArtifact = sut.ReadArtifact();
				Assert.That(secondArtifact[IdentifierFieldName].Value, Is.EqualTo("Record 002"));
				Assert.That(secondArtifact[TextFieldName].Value, Is.EqualTo("Two"));
				Assert.That(sut.CurrentLineNumber, Is.EqualTo(2));

				Assert.That(sut.HasMoreRecords, Is.True);
				var thirdArtifact = sut.ReadArtifact();
				Assert.That(thirdArtifact[IdentifierFieldName].Value, Is.EqualTo("Record 003"));
				Assert.That(thirdArtifact[TextFieldName].Value, Is.EqualTo("Three"));
				Assert.That(sut.CurrentLineNumber, Is.EqualTo(3));

				Assert.That(sut.HasMoreRecords, Is.False);

				// assert - Source identifiers are mapped to CurrentLineNumber
				var firstRecordIdentifier = sut.SourceIdentifierValue(1);
				Assert.That(firstRecordIdentifier, Is.EqualTo("Record 001"));
				var secondRecordIdentifier = sut.SourceIdentifierValue(2);
				Assert.That(secondRecordIdentifier, Is.EqualTo("Record 002"));
				var thirdRecordIdentifier = sut.SourceIdentifierValue(3);
				Assert.That(thirdRecordIdentifier, Is.EqualTo("Record 003"));
			}
		}

		private static DocumentField CreateField(string name, int artifactID, bool isIdentifier)
		{
			int fieldCategoryID = isIdentifier ? (int)FieldCategory.Identifier : (int)FieldCategory.Generic;

			return new DocumentField(
				fieldName: name,
				fieldID: artifactID,
				fieldTypeID: (int)FieldType.Varchar,
				fieldCategoryID: fieldCategoryID,
				codeTypeID: null,
				fieldLength: 255,
				associatedObjectTypeID: null,
				useUnicode: true,
				importBehavior: null,
				guids: new[] { Guid.NewGuid() },
				enableDataGrid: false);
		}
	}
}
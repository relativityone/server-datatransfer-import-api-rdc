Imports System.Text
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Api
Imports NUnit.Framework
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.TestFramework

Namespace Relativity.Desktop.Client.Legacy.NUnit

	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification:="nunit fixture cannot be static")>
	<TestFixture>
	Public Class LoadFileReaderTests
		<Test>
		Public Shared Sub ShouldReadAllRowsFromValidFile()
			Dim expectedIdentifiers = New String() {"AZIPPER_0007299", "AZIPPER_0007300", "AZIPPER_0007301"}
			Dim inputFilePath = ResourceFileHelper.GetResourceFilePath("LoadFileReaderTests", "ValidInput.dat")

			Dim fieldMap = New LoadFileFieldMap()
			fieldMap.Add(GetControlNumberLoadFileFieldMapItem(fieldID:=1, columnInFile:=0))
			fieldMap.Add(GetDateCreatedLoadFileFieldMapItem(fieldID:=2, columnInFile:=1))

			Dim sut As LoadFileReader = CreateAndInitializeSut(inputFilePath, fieldMap)

			' Act
			Dim actualIdentifiers As List(Of String) = New List(Of String)()
			While sut.HasMoreRecords()
				Dim fieldsValues As ArtifactFieldCollection = sut.ReadArtifact()
				actualIdentifiers.Add(fieldsValues.IdentifierField.ValueAsString)
			End While

			' Assert
			CollectionAssert.AreEquivalent(expectedIdentifiers, actualIdentifiers)
		End Sub

		<Test>
		Public Shared Sub SourceIdentifierValue_ShouldBePopulatedWhenParsingFieldWithInvalidDate()
			Const expectedNumberOfRowsWithInvalidDate = 1
			Const expectedIdentifierForInvalidDate = "AZIPPER_0007300"
			Dim inputFilePath = ResourceFileHelper.GetResourceFilePath("LoadFileReaderTests", "InvalidDate.dat")

			Dim fieldMap = New LoadFileFieldMap()
			fieldMap.Add(GetControlNumberLoadFileFieldMapItem(fieldID:=1, columnInFile:=1))
			fieldMap.Add(GetDateCreatedLoadFileFieldMapItem(fieldID:=2, columnInFile:=0))

			Dim sut As LoadFileReader = CreateAndInitializeSut(inputFilePath, fieldMap)

			' Act
			Dim actualNumberOfRowsWithInvalidDate = 0
			While sut.HasMoreRecords()
				Try
					sut.ReadArtifact()
				Catch ex As DateImporterException
					actualNumberOfRowsWithInvalidDate += 1
					Assert.AreEqual(expectedIdentifierForInvalidDate, sut.SourceIdentifierValue())
				End Try
			End While

			' Assert
			Assert.AreEqual(expectedNumberOfRowsWithInvalidDate, actualNumberOfRowsWithInvalidDate)
		End Sub

		<Test>
		Public Shared Sub SourceIdentifierValue_ShouldBeEmptyWhenControlNumberWasEmpty()
			Dim inputFilePath = ResourceFileHelper.GetResourceFilePath("LoadFileReaderTests", "EmptyControlNumber.dat")

			Dim fieldMap = New LoadFileFieldMap()
			fieldMap.Add(GetControlNumberLoadFileFieldMapItem(fieldID:=1, columnInFile:=0))
			fieldMap.Add(GetDateCreatedLoadFileFieldMapItem(fieldID:=2, columnInFile:=1))

			Dim sut As LoadFileReader = CreateAndInitializeSut(inputFilePath, fieldMap)

			' Act
			While sut.HasMoreRecords()
				Dim fieldsValues As ArtifactFieldCollection = sut.ReadArtifact()
				If fieldsValues.IdentifierField.ValueAsString = String.Empty Then
					' Assert
					Assert.AreEqual(String.Empty, sut.SourceIdentifierValue())
				End If
			End While
		End Sub

		Private Shared Function GetControlNumberLoadFileFieldMapItem(ByVal fieldID As Integer, ByVal columnInFile As Integer) As LoadFileFieldMap.LoadFileFieldMapItem
			Dim controlNumberDocumentField = New DocumentField("Control Number", fieldID, FieldType.Varchar, FieldCategory.Identifier, Nothing, 255, Nothing, 1, Nothing, False)
			Dim controlNumberFieldItem = New LoadFileFieldMap.LoadFileFieldMapItem(controlNumberDocumentField, columnInFile)
			Return controlNumberFieldItem
		End Function

		Private Shared Function GetDateCreatedLoadFileFieldMapItem(ByVal fieldID As Integer, ByVal columnInFile As Integer) As LoadFileFieldMap.LoadFileFieldMapItem
			Dim dateCreatedDocumentField = New DocumentField("Date Created", fieldID, FieldType.Date, FieldCategory.Generic, Nothing, Nothing, 1, 1, Nothing, False)
			Dim dateCreatedFieldItem = New LoadFileFieldMap.LoadFileFieldMapItem(dateCreatedDocumentField, columnInFile)
			Return dateCreatedFieldItem
		End Function

		Private Shared Function CreateAndInitializeSut(inputFilePath As Object, fieldMap As Object) As LoadFileReader
			Dim loadFile = New LoadFile() With {
					.FilePath = inputFilePath,
					.FieldMap = fieldMap,
					.CaseInfo = New CaseInfo(),
					.SourceFileEncoding = New UTF8Encoding(),
					.RecordDelimiter = "|",
					.QuoteDelimiter = "^",
					.NewlineDelimiter = Chr(174),
					.FirstLineContainsHeaders = True}

			Dim sut = New LoadFileReader(loadFile, False)

			sut.GetColumnNames(loadFile) 'It is required for proper LoadFile initialization
			If sut.HasMoreRecords() Then
				sut.AdvanceRecord() ' We need to skip first line with column names
			End If

			Return sut
		End Function
	End Class
End Namespace

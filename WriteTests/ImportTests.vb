Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient
Imports kCura.Relativity.DataReaderClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests

	<TestFixture()> _
	Public Class ImportTests
		Inherits WriteTestsBase


#Region "Private Variables"
		Private _errors As System.Collections.Generic.List(Of String)
		Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob
		Private WithEvents DocImportAPI As New Global.kCura.Relativity.DataReaderClient.ImportBulkArtifactJob
		Private sql As String
		Private dataTableSrc As DataTable
		Private dataTableDest As DataTable
		Private destinationFileExists As Boolean
		Private sourceFileSize As Int32
		Private destinationFileSize As Int32
#End Region


#Region "Setup/Tear down"
		''' <summary>
		''' Set up Artifact Test Cases
		''' </summary>
		<SetUp()> _
		Public Sub SetUp()
			Dim helper As New Helpers.SetupHelper()
			_errors = New System.Collections.Generic.List(Of String)
			helper.SetupTestWithRestore()
			Try
				ImportTestsHelper.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			Catch
				ImportTestsHelper.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			End Try
			sourceFileSize = 0
			destinationFileSize = 0
			destinationFileExists = False
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
		End Sub

		''' <summary>
		''' Tear down the test case
		''' </summary>
		''' <remarks></remarks>
		<TearDown()> _
		Public Sub TearDown()
			dataTableSrc.Dispose()
			dataTableDest.Dispose()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
#End Region

#Region "Test Import Image in Append Mode"

		<Test(), _
		Category("HighPriority")> _
		Public Sub ImportImage_Append_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "select Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInDestination()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "select Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000042') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Document does not exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are the same size (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInDestination_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000043') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInDestination_Negate_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "select '' As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000043') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.False(destinationFileExists, "File does not exists in destination repository (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInSource_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000044') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

#End Region

#Region "Test Import Image in AppendOverlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000045') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000046') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_Redacted()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000053') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_RedactedWithText()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000054') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_Highlighted()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000055') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select '' As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000046') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000047') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_Negate_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select '' As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000047') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.False(destinationFileExists, "File does not exists in destination repository (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInSource_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000048') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		

#End Region

#Region "Test Import Image in Overlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000049') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000050') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_Redacted()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000053') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_RedactedWithText()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000054') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_Highlighted()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000055') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select '' As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000050') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000051') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInSource_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000052') "
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc
			' Act
			ImportAPI.Execute()
			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub

	End Class
End Namespace

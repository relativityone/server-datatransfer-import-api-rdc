Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.Data.SqlClient
Imports kCura.Relativity.DataReaderClient


Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class ImportImageWithExtractedText
		Inherits WriteTestsBase

#Region "Private Variables"
		Private _errors As System.Collections.Generic.List(Of String)
		Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob
		'Private WithEvents DocImportAPI As New Global.kCura.Relativity.DataReaderClient.ImportBulkArtifactJob
		Private sql As String
		Private dataTableSrc As DataTable
		Private dataTableDest As DataTable
		Private destinationFileExists As Boolean
		Private sourceFileSize, destinationFileSize As Int32
		Private sourceExtractedText, destinationExtractedText As String
#End Region

#Region "Setup/Tear down"
		''' <summary>
		''' Set up Artifact Test Cases
		''' </summary>
		<SetUp()> _
		Public Overrides Sub SetUp()
			MyBase.SetUp()
			_errors = New System.Collections.Generic.List(Of String)
			Try
				ImportTestsHelper.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			Catch
				ImportTestsHelper.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			End Try
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
			sql = String.Empty
			sourceFileSize = 0
			destinationFileSize = 0
			destinationFileExists = False
			sourceExtractedText = String.Empty
			destinationExtractedText = String.Empty
		End Sub

		<TearDown()> _
		Public Overrides Sub TearDown()
			MyBase.TearDown()
			dataTableSrc.Dispose()
			dataTableDest.Dispose()
		End Sub
#End Region



#Region "Test Import Image With Extracted Text in Append Mode"

		<Test(), _
		Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
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
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination()
			' Arrange		
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Document does not exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are the same size (incorrect).")
			Assert.AreNotEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination_Negate()
			' Arrange			
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination_Negate_NoIdentifier()
			' Arrange			
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "SELECT '' AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
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
		Public Sub ImportImageWithExtractedText_Append_FileExistsInSource_Negate()
			' Arrange			
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
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
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in AppendOverlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
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
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(0, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT '' AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreNotEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "Documents were not imported (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination_Negate_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT '' AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
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
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInSource_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.AppendOverlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
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
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in Overlay Mode"
		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_NotSupportedImageFile()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
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
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination_NoIdentifier()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "SELECT '' AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			' Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationExtractedText = CType(dataTableDest.Rows(0)("ExtractedText"), String)
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			sourceExtractedText = CType(dataTableSrc.Rows(0)("ExtractedText"), String)
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreEqual(1, dataTableDest.Rows.Count, "No documents  exists in destination (incorrect).")
			Assert.True(destinationFileExists, "File does not exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are not the same size (incorrect).")
			Assert.AreNotEqual(sourceExtractedText, destinationExtractedText, "Source and destination ExtractedText are the same (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataTableSrc = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataTableSrc

			'Act
			ImportAPI.Execute()

			' Assert
			dataTableDest = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If dataTableDest.Rows.Count > 0 Then
				destinationFileExists = System.IO.File.Exists(CType(dataTableDest.Rows(0)("FileLocation"), String))
				destinationFileSize = ImportTestsHelper.GetFileSize(CType(dataTableDest.Rows(0)("FileLocation"), String))
			End If
			sourceFileSize = ImportTestsHelper.GetFileSize(CType(dataTableSrc.Rows(0)("FileLocation"), String))
			Assert.AreNotEqual(0, _errors.Count, "Import failed.")
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
			Assert.AreNotEqual(sourceFileSize, destinationFileSize, "Source and destination files are the same size (incorrect).")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInSource_Negate()
			' Arrange
			ImportAPI.Settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Overlay
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier], ExtractedText FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	 " WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
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
			Assert.AreNotEqual(1, dataTableDest.Rows.Count, "Documents were imported (incorrect).")
			Assert.False(destinationFileExists, "File exists in destination repository (incorrect).")
		End Sub
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub

	End Class
End Namespace
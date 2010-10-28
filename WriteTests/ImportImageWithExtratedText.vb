Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.Data.SqlClient
Imports kCura.Relativity.DataReaderClient


Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class ImportImageWithExtratedText
		Inherits WriteTestsBase

#Region "Private Variables"
		Private _errors As System.Collections.Generic.List(Of String)
		Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob
		Private WithEvents DocImportAPI As New Global.kCura.Relativity.DataReaderClient.ImportBulkArtifactJob
		Private sql As String
		Private dataReader As IDataReader
		Private dataTable As DataTable
		Private fileExists As Boolean
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
		<TearDown()> _
		Public Sub TearDown()
			dataReader.Close()
			dataReader.Dispose()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
#End Region



#Region "Test Import Image With Extracted Text in Append Mode"

		<Test(), _
		Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
 " WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(Sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "There were error while importing Bad Image With Extracted Text in Append Mode")
			Assert.AreEqual(0, dataTable.Rows.Count, "Documents were not imported correctly using Import Image with Extracted Text in Append Mode")
			Assert.AreEqual(0, dataTable.Rows.Count, "Documents were not imported correctly using Import Image with Extracted Text in Append Mode")
			Assert.False(fileExists, "File was not copied ")
		End Sub


		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination()
			'Arrange		
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			sql = "SELECT [File].[Identifier] AS [BatesNumber], [File].[Location] AS [FileLocation], [File].[DocumentArtifactID] AS [DocumentIdentifier], [Document].[ExtractedText] FROM [File] " + _
		 " INNER JOIN [Document] on [File].[Identifier] =  [Document].[ControlNumber] WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
			Assert.True(fileExists, "File was not copied ")
		End Sub



		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination_Negate()
			'Arrange			
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
			Assert.True(fileExists, "File was not copied ")
		End Sub


		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_NoImageFile()
			'Arrange			
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in AppendOverlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.True(fileExists, "File was not copied ")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.True(fileExists, "File was not copied ")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in Overlay Mode"
		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_001') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_002') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.True(fileExists, "File was not copied ")
		End Sub


		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_003') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "SELECT Identifier AS [BatesNumber], Location AS [FileLocation], DocumentArtifactID AS [DocumentIdentifier]  FROM [File] INNER JOIN [Document] ON [File].[Identifier] =  [Document].[ControlNumber] " + _
	" WHERE [Identifier] IN ('Image_004') AND [Document].[ExtractedText] IS NOT NULL"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			If Not String.IsNullOrEmpty(dataTable.Rows(0)("Location")) Then fileExists = ImportTestsHelper.DetermineIfFileExists(dataTable.Rows(0)("Location"))
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
			Assert.False(fileExists, "File was not copied ")
		End Sub
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub

	End Class
End Namespace
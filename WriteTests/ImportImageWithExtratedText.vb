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
		End Sub

		''' <summary>
		''' Tear down the test case
		''' </summary>
		<TearDown()> _
		Public Sub TearDown()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
#End Region



#Region "Test Import Image With Extracted Text in Append Mode"

		<Test(), _
		Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_BadImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_001')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import Image With Extracted Text in Append Mode was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "Documents are not correctly imported using Import Image With Extracted Text in Append Mode")
		End Sub


		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_002')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub



		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_003')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub


		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Append_NoImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_004')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in AppendOverlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_001')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_002')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_003')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_AppendOverlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_004')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

#End Region

#Region "Test Import Image With Extracted Text in Overlay Mode"
		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_001')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_002')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub


		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_003')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImageWithExtractedText_Overlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('Image_004')"
			Dim dataReader As IDataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			dataReader.Close()
			dataReader.Dispose()
			' Assert
			Dim dataTable As DataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub

	End Class
End Namespace
Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
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
		Private dataReader As IDataReader
		Private dataTable As DataTable
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
				''' <remarks></remarks>
		<TearDown()> _
		Public Sub TearDown()
			dataReader.Close()
			dataReader.Dispose()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
		#End Region

#Region "Test Import Image in Append Mode"

		<Test(), _
		Category("HighPriority")> _
		Public Sub ImportImage_Append_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Append mode was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub

		
		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000042') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Append mode was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub


		
		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000043') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import in Append mode was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub

		
		<Test(), _
		 Category("HighPriority")> _
		Public Sub ImportImage_Append_NoImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.none
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000044') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Append mode was not successful. There are errors")
			Assert.AreNotEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub

#End Region

#Region "Test Import Image in AppendOverlay Mode"

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000045') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in AppendOverlay mode was not successful. There are errors")
			Assert.AreEqual(0, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000046') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import in AppendOverlay mode was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000047') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import in AppendOverlay mode was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_AppendOverlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.both
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000048') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in AppendOverlay mode was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

#End Region

#Region "Test Import Image in Overlay Mode"
		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_BadImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000049') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Overlay mode was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000050') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import in Overlay mode was not successful. There are errors")
			Assert.AreEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub


		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_FileExistsInDestination_Negate()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000051') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Overlay mode was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		<Test(), Category("HighPriority")> _
		Public Sub ImportImage_Overlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.OverwriteMode = ImportTestsHelper.OverwriteModeEnum.strict
			sql = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000052') Order By [BatesNumber]"
			dataReader = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			dataTable = ImportTestsHelper.ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import in Overlay mode was not successful. There are errors")
			Assert.AreNotEqual(1, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub
#End Region



#Region "Document Import Test"
		''''<summary>
		'''' ImportImage Append
		'''' </summary>
		'<Test(), _
		' Category("HighPriority"), _
		'Description("Verify that import test works")> _
		'Public Sub ImportDocument_Append()
		'	' Arrange
		'	DocImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
		'	DocImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
		'	DocImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
		'	DocImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
		'	' ? DocImportAPI.Settings.DestinationFolderArtifactID = Helpers.CommonDefaults.
		'	' ? DocImportAPI.Settings.MultiValueDelimiter = 
		'	' ? DocImportAPI.Settings.NestedValueDelimiter = 
		'	DocImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Append
		'	DocImportAPI.Settings.CopyFilesToDocumentRepository = True
		'	DocImportAPI.Settings.ForProduction = False
		'	DocImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
		'	DocImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
		'DocImportAPI.Settings.FolderPathSourceFieldName = 
		'DocImportAPI.Settings.ParentObjectIdSourceFieldName =
		'	DocImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
		'	DocImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
		'	sql  = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041','FED000042') Order By [BatesNumber]"
		'	dataReader  = ImportTestsHelper.ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
		'	ImportAPI.SourceData.SourceData = dataReader
		'	ImportAPI.Execute()
		'	' Assert
		'	dataTable  = ImportTestsHelper.ExecuteSQLStatementAsDataTablesql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
		'	Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
		'	Assert.AreEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
		'End Sub
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub

	End Class
End Namespace

Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.Data.SqlClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class ImportTests
		Inherits WriteTestsBase
		Private _errors As System.Collections.Generic.List(Of String)

#Region "Private Variables"
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
				Me.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			Catch
				Me.ExecuteSQLStatementAsDataTable("select top 1 artifactid from artifact", -1)
			End Try
		End Sub

		''' <summary>
		''' Tear down the test case
		''' </summary>
		''' <remarks></remarks>
		<TearDown()> _
		Public Sub TearDown()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
#End Region

#Region "Tests"

		'''<summary>
		''' ImportImage Append
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub ImportImage_Append()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Append
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041','FED000042') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub


		'''<summary>
		''' ImportImage Overlay
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub ImportImage_Overlay()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Overlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000043','FED000044') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		'''<summary>
		''' ImportImage AppendOverlay
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub ImportImage_AppendOverlay()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000042','FED000044') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		'''<summary>
		''' ImportImage AppendOverlay
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub Negate_ImportImage_AppendOverlay_NoImageFile()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation],  DocumentArtifactID  as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000045','FED000046') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		'''<summary>
		''' ImportImage AppendOverlay
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub Negate_ImportImage_AppendOverlay_BadImageFiles()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			ImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			ImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000047','FED000048') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreNotEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreNotEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub
#End Region

#Region "Document Import Test"
		'''<summary>
		''' ImportImage Append
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub ImportImage_Append()
			'Arrange
			DocImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			DocImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			DocImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION
			DocImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			DestinationFolderArtifactID()
			MultiValueDelimiter()
			NestedValueDelimiter()
			DocImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Append
			DocImportAPI.Settings.CopyFilesToDocumentRepository = True
			DocImportAPI.Settings.ForProduction = False
			DocImportAPI.Settings.IdentityFieldId = Helpers.CommonDefaults.IDENTITY_FIELD_ID
			DocImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			FolderPathSourceFieldName()
			ParentObjectIdSourceFieldName()
			DocImportAPI.Settings.ExtractedTextFieldContainsFilePath = True
			DocImportAPI.Settings.ExtractedTextEncoding = System.Text.Encoding.UTF8
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041','FED000042') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_SOURCE)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_IMPORT_API_DESTINATION)
			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "Documents are not correctly imported")
		End Sub
#End Region

#Region " Private Methods "

		Public Function ExecuteSQLStatementAsDataTableAsDataReader(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.IDataReader
			Try
				Return Me.ExecuteReader(sqlStatement, caseArtifactID)
			Catch
				Return Me.ExecuteReader(sqlStatement, caseArtifactID)
			End Try
		End Function

		Private Function ExecuteReader(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As IDataReader
			Dim command As New SqlCommand
			Dim resultReader As SqlDataReader
			command.CommandText = sqlStatement
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", "EDDS" & caseArtifactID)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			resultReader = command.ExecuteReader()
			Return resultReader
		End Function

		Public Function ExecuteSQLStatementAsDataTable(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.DataTable
			Dim dataAdapter As System.Data.SqlClient.SqlDataAdapter
			Dim dataTable As System.Data.DataTable
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = "EDDS"
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			dataAdapter = New System.Data.SqlClient.SqlDataAdapter(command)
			dataTable = New System.Data.DataTable
			dataAdapter.Fill(dataTable)
			Return dataTable
		End Function
#End Region

		Private Sub ImportAPI_OnMessage(ByVal status As Status) Handles ImportAPI.OnMessage
			Console.WriteLine(status.Message)
			If status.Message.ToLower.Contains("error") Then _errors.Add(status.Message)
		End Sub
	End Class
End Namespace

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
		'Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImportBulkArtifactJob
		Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob
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
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT
			ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Append
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = 1003667
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000041','FED000042') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_CRUD)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			'Count doc in desti.
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT)
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
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Overlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = 1003667
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000043','FED000044') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_CRUD)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			'Count doc in desti.
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT)

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
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT
			ImportAPI.Settings.ArtifactTypeId = 10
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = 1003667
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000042','FED000044') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_CRUD)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			'Count doc in desti.
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT)

			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub

		'<Test(), _
		'Category("HighPriority"), _
		'Description("Successful import with extracted text - Append")> _
		'Public Sub ImportTest2()
		'	ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
		'	ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
		'	ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_CRUD
		'	ImportAPI.Settings.ArtifactTypeId = Helpers.CommonDefaults.DOCTYPEID

		'End Sub


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
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT
			ImportAPI.Settings.ArtifactTypeId = 10
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = 1003667
			'ImportAPI.Settings.
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000045','FED000046') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_CRUD)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			'Count doc in desti.
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT)

			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
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
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT
			ImportAPI.Settings.ArtifactTypeId = 10
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay
			ImportAPI.Settings.CopyFilesToDocumentRepository = True
			ImportAPI.Settings.ForProduction = False
			ImportAPI.Settings.IdentityFieldId = 1003667
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim sql As String = "select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] WHERE [Identifier] IN ('FED000047','FED000048') Order By [BatesNumber]"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader(sql, Helpers.CommonDefaults.CASE_ID_CRUD)
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()
			' Assert
			'Count doc in desti.
			Dim dataTable As DataTable = ExecuteSQLStatementAsDataTable(sql, Helpers.CommonDefaults.CASE_ID_QUERY_BATCH_SCRIPT)

			Assert.AreEqual(0, _errors.Count, "Import was not successful. There are errors")
			Assert.AreEqual(2, dataTable.Rows.Count, "documents are not correctly imported")
		End Sub
#End Region

#Region " Private Methods "

		Public Function ExecuteSQLStatementAsDataTableAsDataReader(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.IDataReader
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096", "EDDS" & caseArtifactID)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			Return command.ExecuteReader()
		End Function

		Public Function ExecuteSQLStatementAsDataTable(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.DataTable
			Dim dataAdapter As System.Data.SqlClient.SqlDataAdapter
			Dim dataTable As System.Data.DataTable
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096", "EDDS" & caseArtifactID)
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

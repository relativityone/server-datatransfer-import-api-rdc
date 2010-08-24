Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.Data.SqlClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class ImportTests
		Inherits WriteTestsBase
#Region "Private Variables"
		Private WithEvents ImportAPI As New Global.kCura.Relativity.DataReaderClient.ImportBulkArtifactJob
#End Region

#Region "Setup/Tear down"
		''' <summary>
		''' Set up Artifact Test Cases
		''' </summary>
		<SetUp()> _
		Public Sub SetUp()
			Dim helper As New Helpers.SetupHelper()
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
		''' Import Test
		''' </summary>
		<Test(), _
		 Category("HighPriority"), _
		Description("Verify that import test works")> _
		Public Sub ImportTest1()
			'Arrange
			ImportAPI.Settings.RelativityUsername = Helpers.CommonDefaults.API_USER_ADMIN
			ImportAPI.Settings.RelativityPassword = Helpers.CommonDefaults.API_USER_ADMIN_PASSWORD
			ImportAPI.Settings.CaseArtifactId = Helpers.CommonDefaults.CASE_ID_CRUD
			ImportAPI.Settings.ArtifactTypeId = 10
			ImportAPI.Settings.OverwriteMode = OverwriteModeEnum.Overlay
			ImportAPI.Settings.OverlayIdentifierSourceFieldName = "Control Number"
			Dim dataReader As IDataReader = ExecuteSQLStatementAsDataTableAsDataReader("select Identifier As [BatesNumber], Location As [FileLocation], DocumentArtifactID as [DocumentIdentifier]  from [File] Order By [BatesNumber]")
			ImportAPI.SourceData.SourceData = dataReader
			ImportAPI.Execute()

		End Sub

#End Region

#Region " Private Methods "

		Public Function ExecuteSQLStatementAsDataTableAsDataReader(ByVal sqlStatement As String) As System.Data.IDataReader
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			command.Connection = New SqlConnection("data source=localhost\integratedtests;initial catalog=EDDS1016506;persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096")
			command.Connection.Open()
			Return command.ExecuteReader()
		End Function

#End Region
	End Class
End Namespace

Imports System.Net
Imports Rhino.Mocks
Imports NUnit.Framework
Imports kCura.WinEDDS.Service
Imports kCura.WinEDDS
Imports kCura.WinEDDS.NUnit

Namespace kCura.WinEDDS.NUnit.Services
	<TestFixture()> Public Class BulkImportManagerTests

#Region " Members "
		Dim _testObj As kCura.WinEDDS.Service.BulkImportManager = Nothing
		Private _mockRepo As MockRepository = Nothing
		Dim _mockCreddentials As ICredentials
		Dim _mockCookieContainer As CookieContainer
		Const ServiceUrlPageFormat As String = "{0}BulkImportManager.asmx"
		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
#End Region

#Region "Setup And Teardown"

		<SetUp()> Public Sub SetUp()
			_mockRepo = New MockRepository()
			_mockCreddentials = _mockRepo.DynamicMock(Of ICredentials)()
			_mockCookieContainer = _mockRepo.DynamicMock(Of CookieContainer)()

			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If
		End Sub

		<TearDown()> Public Sub TearDown()
			_testObj = Nothing
			_mockCreddentials = Nothing
			_mockCookieContainer = Nothing
			_mockRepo = Nothing

			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub

#End Region

	End Class
End Namespace
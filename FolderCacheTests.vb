Imports NUnit.Framework
Imports Rhino.Mocks
Imports kCura.WinEDDS.Service

'Todo: this tests need cleaning and functionality
Namespace kCura.WinEDDS.NUnit

	'<TestFixture()> 
	Public Class FolderCacheTests

#Region " Members "
		Private _mockRepo As MockRepository = Nothing
		Dim _mockFolderManager As FolderManager
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			_mockRepo = New MockRepository()
			_mockFolderManager = _mockRepo.DynamicMock(Of FolderManager)()
		End Sub

#End Region

	End Class

End Namespace

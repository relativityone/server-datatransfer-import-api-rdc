Imports NUnit.Framework
Imports kCura.Windows.Process

Namespace kCura.WinEDDS.NUnit

	'<TestFixture()> 
	Public Class ImageFilePreviewerTests

#Region " Members "
		Dim _args As ImageLoadFile
		Dim _controller As Controller
		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			_args = New ImageLoadFile()
			_args.CaseInfo = New Relativity.CaseInfo()
			_args.CaseInfo.RootArtifactID = 1
			_controller = New Controller()

			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If
		End Sub

		<TearDown()> Public Sub TakeDown()
			_args = Nothing
			_controller = Nothing

			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub
#End Region

	End Class
End Namespace

Imports Microsoft.Win32

Namespace kCura.WinEDDS.NUnit
	Public Class RegKeyHelper
		Public Shared ReadOnly Property RelativityKeyPath() As String
			Get
				Return "software\kCura\Relativity"
			End Get
		End Property

		Public Shared ReadOnly Property RelativityServiceURLKey() As String
			Get
				Return "WebServiceURL"
			End Get
		End Property

		Public Shared ReadOnly Property RelativityDefaultServiceURL() As String
			Get
				Return "http://localhost/relativitywebapi/"
			End Get
		End Property

		Public Shared Function SubKeyPathExists(ByVal keyPath As String) As Boolean
			Try
				Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath, False)
				Dim keyExists As Boolean = False

				If Not regKey Is Nothing Then
					keyExists = True
				End If

				Return keyExists
			Catch ex As Exception
				Dim msg As String = String.Format("Exception in SubKeyPathExists. Input={1}{0} Message={2}{0} StackTrace={3}", vbNewLine, keyPath, ex.Message, ex.StackTrace)
				Throw New Exception(msg)
			End Try
		End Function

		Public Shared Function SubKeyExists(ByVal keyPath As String, ByVal keyName As String) As Boolean
			Try
				Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath, False)
				Dim keyVal As Object = regKey.GetValue(keyName)
				Dim valExists As Boolean = False

				If Not keyVal Is Nothing Then
					valExists = True
				End If

				Return valExists
			Catch ex As Exception
				Dim msg As String = String.Format("Exception in SubKeyExists. Input keyPath={1}{0} Input keyName={2}{0} Message={3}{0} StackTrace={4}", vbNewLine, keyPath, keyName, ex.Message, ex.StackTrace)
				Throw New Exception(msg)
			End Try
		End Function

		Public Shared Sub CreateKeyWithValueOnPath(ByVal createPath As Boolean, ByVal keyPath As String, ByVal keyName As String, ByVal keyVal As String)
			Try
				If createPath = True Then
					Registry.CurrentUser.CreateSubKey(keyPath)
				End If

				Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath, True)
				regKey.SetValue(keyName, keyVal, RegistryValueKind.String)
			Catch ex As Exception
				Dim msg As String = String.Format("Exception in CreateKeyWithValueOnPath. Input createPath={1}{0} Input keyPath={2}{0} Input keyName={3}{0} Input keyVal={4}{0} Message={5}{0} StackTrace={6}", vbNewLine, createPath, keyPath, keyName, keyVal, ex.Message, ex.StackTrace)
				Throw New Exception(msg)
			End Try
		End Sub

		Public Shared Sub RemoveKeyPath(ByVal keyPath As String)
			Try
				Registry.CurrentUser.DeleteSubKeyTree(keyPath, False)
			Catch ex As Exception
				Dim msg As String = String.Format("Exception in RemoveKeyPath. Input={1}{0} Message={2}{0} StackTrace={3}", vbNewLine, keyPath, ex.Message, ex.StackTrace)
				Throw New Exception(msg)
			End Try
		End Sub
	End Class
End Namespace
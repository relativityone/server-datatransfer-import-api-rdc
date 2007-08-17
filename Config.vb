Imports System.Configuration
Namespace kCura.Windows.Process
	Public Class Config

		Private Shared _configDictionary As System.Collections.IDictionary

		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				Try
					If _configDictionary Is Nothing Then
						_configDictionary = DirectCast(System.Configuration.ConfigurationSettings.GetConfig("kCura.Windows.Process"), System.Collections.IDictionary)
					End If
					Return _configDictionary
				Catch ex As System.Exception
					Return Nothing
				End Try
			End Get
		End Property

		Public Shared ReadOnly Property LogAllEvents() As Boolean
			Get
				Try
					Return CType(ConfigSettings("LogAllEvents"), Boolean)
				Catch ex As Exception
					Return False
				End Try
			End Get
		End Property

	End Class
End Namespace

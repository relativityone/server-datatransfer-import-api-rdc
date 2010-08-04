Imports System.Configuration
Namespace kCura.Windows.Process
	Public Class Config

		Private Shared _configDictionary As System.Collections.IDictionary

		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				Try
					If _configDictionary Is Nothing Then
						_configDictionary = DirectCast(System.Configuration.ConfigurationManager.GetSection("kCura.Windows.Process"), System.Collections.IDictionary)
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
					If ConfigSettings IsNot Nothing AndAlso ConfigSettings.Contains("LogAllEvents") AndAlso Type.GetTypeCode(ConfigSettings("LogAllEvents").GetType()).Equals(System.TypeCode.Boolean) Then
						Return CType(ConfigSettings("LogAllEvents"), Boolean)
					Else
						Return False
					End If
				Catch ex As Exception
					Return False
				End Try
			End Get
		End Property

	End Class
End Namespace

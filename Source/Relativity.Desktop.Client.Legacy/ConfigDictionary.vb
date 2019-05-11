Imports Relativity.DataExchange

Namespace Relativity.Desktop.Client
	Friend Class ConfigDictionary
		Inherits DictionaryBase
		Private ReadOnly _application As Application
		Private _hasInitialized As Boolean = False
		Friend Sub New(ByVal sectionName As String, ByVal valuesCollection As Collection)
			MyBase.New(sectionName, valuesCollection)
			_application = Application.Instance
		End Sub

		Protected Overrides Sub UpdateValues()
			' This is a good opportunity to refresh all settings.
			' Intentionally NOT refreshing within the task below to eliminate possible race conditions and relate issues.
			AppSettingsManager.Refresh(AppSettings.Instance)
			Dim configTable As System.Data.DataTable
			Try
				configTable = Task.Run(Async Function()
										   Return Await _application.GetSystemConfiguration().ConfigureAwait(False)
									   End Function).Result
				_hasInitialized = True
			Catch
				If Not _hasInitialized Then
					Throw
				Else
					configTable = New System.Data.DataTable
				End If
			End Try
			For Each row As System.Data.DataRow In configTable.Rows
				Dim section As String = CType(row("Section"), String)
				Dim key As String = (CType(row("Name"), String))
				Dim value As String = CType(row("Value"), String)
				If _valuesCollection.SectionHash(section) Is Nothing Then
					_valuesCollection.SectionHash(section) = New System.Collections.Hashtable
				End If
				CType(_valuesCollection.SectionHash(section), System.Collections.Hashtable)(key) = value
			Next
		End Sub

	End Class
End Namespace
Namespace kCura.EDDS.WinForm
	Friend Class ConfigDictionary
		Inherits kCura.Config.DictionaryBase
		Private _application As kCura.EDDS.WinForm.Application
		Private _hasInitialized As Boolean = False
		Friend Sub New(ByVal sectionName As String, ByVal valuesCollection As kCura.Config.Collection)
			MyBase.New(sectionName, valuesCollection)
			_application = kCura.EDDS.WinForm.Application.Instance
		End Sub

		Protected Overrides Sub UpdateValues()
			Dim configTable As System.Data.DataTable
			Try
				configTable = _application.GetSystemConfiguration
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


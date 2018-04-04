Namespace kCura.WinEDDS
	Public Interface IFieldService
		Inherits IFieldLookupService
		
		Function GetColumns() As ViewFieldInfo()

		Function GetColumnHeader() As String

	End Interface
End Namespace

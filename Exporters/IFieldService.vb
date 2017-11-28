Namespace kCura.WinEDDS
	Public Interface IFieldService
		Inherits IFieldLookupService

		Function GetColumns() As ArrayList

		Function GetColumnHeader() As String

	End Interface
End Namespace

Namespace kCura.WinEDDS
	Public Interface IFieldService
		Inherits IFieldLookupService

		'TODO change type to ViewFieldInfo
		Function GetColumns() As ArrayList

		Function GetColumnHeader() As String

	End Interface
End Namespace

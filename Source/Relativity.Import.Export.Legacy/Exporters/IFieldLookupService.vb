Namespace kCura.WinEDDS

	Public Interface IFieldLookupService

		Function GetOrdinalIndex(fieldName As String) As Int32

		Function ContainsFieldName(fieldName As String) As Boolean

	End Interface


End Namespace


Namespace Relativity.Desktop.Client
	''' <summary>
	''' Represents a key/value store of SQL-backed configuration values.
	''' </summary>
	Public Class Dictionary
		Inherits DictionaryBase

		''' <summary>
		''' Initializes a new instance of Dictionary with the given section name and collection of values.
		''' </summary>
		''' <param name="sectionName">The name of the section these configuration values represent.</param>
		''' <param name="valuesCollection">The collection of configuration values to be used.</param>
		Public Sub New(ByVal sectionName As String, ByVal valuesCollection As Collection)
			MyBase.New(sectionName, valuesCollection)
		End Sub

		Protected Overrides Sub UpdateValues()
			' The RDC doesn't use direct SQL.
		End Sub
	End Class
End Namespace
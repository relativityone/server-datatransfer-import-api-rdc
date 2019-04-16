Namespace Relativity.Desktop.Client

	''' <summary>
	''' Provides a factory method for retrieving DictionaryBase objects.
	''' </summary>
	Public Interface IConfigDictionaryFactory

		''' <summary>
		''' Gets a DictionaryBase of configuration values for the given section name and Collection.
		''' </summary>
		''' <param name="sectionName">
		''' The section name of the DictionaryBase to get.
		''' </param>
		''' <param name="collection">
		''' The Collection to be used by the returned DictionaryBase.
		''' </param>
		''' <returns>
		''' The DictionaryBase instance.
		''' </returns>
		Function GetDictionary(ByVal sectionName As String, ByVal collection As Collection) As DictionaryBase
	End Interface
End Namespace
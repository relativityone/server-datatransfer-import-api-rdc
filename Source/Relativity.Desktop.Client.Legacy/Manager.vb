Imports System.Configuration

Namespace Relativity.Desktop.Client

	''' <summary>
	''' Provides methods for parse-safe retrieval, encryption/decryption, and storage of configuration values.
	''' </summary>
	Public Class Manager
		Private Shared _valuesCollection As Collection

		Private Shared ReadOnly Property GetConfigCollection() As Collection
			Get
				If _valuesCollection Is Nothing Then
					_valuesCollection = New Collection
				End If
				Return _valuesCollection
			End Get
		End Property

		''' <summary>
		''' Gets an IDictionary of configuration values with the given section name using the given dictionary factory.
		''' </summary>
		''' <param name="sectionName">The section name with which to retrieve configuration values.</param>
		''' <param name="dictionaryFactory">The IConfigDictionaryFactory with which to retrieve an IDictionary.</param>
		''' <returns>An IDictionary of configuration values with the given section name.</returns>
		''' <exception cref="ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		Public Shared Function GetConfig(ByVal sectionName As String, ByVal dictionaryFactory As IConfigDictionaryFactory) As IDictionary
			Dim d As DictionaryBase = dictionaryFactory.GetDictionary(sectionName, GetConfigCollection)
			If d Is Nothing Then
				Return DirectCast(System.Configuration.ConfigurationManager.GetSection(sectionName), System.Collections.IDictionary)
			Else
				Return d
			End If
		End Function

		''' <summary>
		''' Gets from the given IDictionary the element specified by the given key, or a default value if the key does not exist. 
		''' </summary>
		''' <param name="settings">The IDictionary to search.</param>
		''' <param name="key">The key of the element to get.</param>
		''' <param name="defaultValue">The value to be returned if the key is not present.</param>
		''' <returns>The element specified by the given key, or a default value if it does not exist.</returns>
		Public Shared Function GetValue(ByVal settings As IDictionary, ByVal key As String, ByVal defaultValue As Object) As Object
			Dim obj As Object = Nothing
			If settings Is Nothing Then
				Return defaultValue
			End If

			Try
				obj = settings(key)
			Catch ex As ConfigurationException
				Throw
			Catch ex As System.Exception
				'Do Nothing
			End Try
			If obj Is Nothing Then
				Return defaultValue
			Else
				Return obj
			End If
		End Function
	End Class
End Namespace
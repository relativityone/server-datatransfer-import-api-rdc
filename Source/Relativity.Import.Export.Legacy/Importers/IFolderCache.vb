Namespace kCura.WinEDDS.Importers

	''' <summary>
	''' Represents an abstract client-side folder cache.
	''' </summary>
	Public Interface IFolderCache

		''' <summary>
		''' Gets the total number of folders in the cache.
		''' </summary>
		''' <value>
		''' The total count.
		''' </value>
		ReadOnly Property Count As Int32

		''' <summary>
		''' Retrieves the folder artifact identifier for the specified folder path and automatically create all sub-folder paths that don't already exist.
		''' </summary>
		''' <param name="folderPath">
		''' The path used to lookup the folder artifact identifier.
		''' </param>
		''' <returns>
		''' The folder artifact identifier.
		''' </returns>
		''' <exception cref="kCura.WinEDDS.Exceptions.WebApiException">
		''' Thrown when a failure occurs retrieving or creating folders.
		''' </exception>
		Function GetFolderId(ByVal folderPath As String) As Int32
	End Interface
End NameSpace
Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Contains source data for import.
	''' </summary>
	Public Class ImageSourceIDataReader
		Private _sourceData As System.Data.DataTable

		''' <summary>
		''' Represents an instance of the ImageSourceIDataReader, which contains data for import. This property is required.
		''' </summary>
		''' <remarks>For standard imports, the ImageSourceIDataReader operates as an iterator over a DataTable instance that contains the data source.</remarks>
		Public Property SourceData() As System.Data.DataTable
			Get
				Return _sourceData
			End Get
			Set(ByVal Value As System.Data.DataTable)
				_sourceData = Value
				'Load the first records in the reader. For some reason we need to do this for document load. This will read one record before processing the rest. Why do we need to do this?
				'_sourceData.Read()
			End Set
		End Property

	End Class

End Namespace
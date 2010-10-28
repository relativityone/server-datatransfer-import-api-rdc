Namespace kCura.Relativity.DataReaderClient

	Public Class ImageSourceIDataReader
		Private _sourceData As System.Data.DataTable

		''' <summary>
		''' System.Data.IDataReader containing source data
		''' </summary>
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
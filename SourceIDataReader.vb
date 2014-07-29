Namespace kCura.Relativity.DataReaderClient

	Public Class SourceIDataReader
		Private _sourceData As System.Data.IDataReader

		''' <summary>
		''' Represents an instance of the SourceIDataReader, which contains data for import. This property is required.
		''' </summary>
		''' <remarks>For standard imports, the SourceIDataReader requires a generic IDataReader object, and operates as an iterator over a DataTable instance that contains the data source.</remarks>
		Public Property SourceData() As System.Data.IDataReader
			Get
				Return _sourceData
			End Get
			Set(ByVal Value As System.Data.IDataReader)
				_sourceData = Value
				'Load the first records in the reader.
				_sourceData.Read()
			End Set
		End Property

	End Class

End Namespace
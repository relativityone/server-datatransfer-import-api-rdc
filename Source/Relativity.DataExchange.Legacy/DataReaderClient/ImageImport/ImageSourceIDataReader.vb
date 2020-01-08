Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Contains source data for import.
	''' </summary>
	Public Class ImageSourceIDataReader
		Private _sourceData As System.Data.DataTable
		Private _reader As IDataReader

		''' <summary>
		''' Represents an instance of the ImageSourceIDataReader, which contains data for import. One of Reader and SourceData properties is required. If both are provided Reader is used.
		''' </summary>
		''' <remarks>For standard imports, the ImageSourceIDataReader operates as an iterator over a DataTable instance that contains the data source.</remarks>
		Public Property SourceData() As System.Data.DataTable
			Get
				Return _sourceData
			End Get
			Set(ByVal Value As System.Data.DataTable)
				_sourceData = Value
				_reader = _sourceData.CreateDataReader()
			End Set
		End Property

        ''' <summary>
        ''' Represents an instance of the ImageSourceIDataReader, which contains data for import. One of Reader and SourceData properties is required. If both are provided Reader is used.
        ''' </summary>
        ''' <remarks>For standard imports, the ImageSourceIDataReader operates as an iterator over a IDataReader instance that contains the data source.</remarks>
        Public Property Reader() As IDataReader
            Get
                Return _reader
            End Get
            Set(ByVal Value As IDataReader)
                _reader = Value
            End Set
        End Property

	End Class

End Namespace
Namespace kCura.Relativity.DataReaderClient

	Public Class SourceIDataReader
		Private _sourceData As System.Data.IDataReader

		''' <summary>
		''' System.Data.IDataReader containing source data
		''' </summary>
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
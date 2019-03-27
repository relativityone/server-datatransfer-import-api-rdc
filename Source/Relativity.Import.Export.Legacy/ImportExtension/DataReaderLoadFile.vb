Namespace kCura.WinEDDS.ImportExtension

	Public Class DataReaderLoadFile
		Inherits kCura.WinEDDS.LoadFile

		Private _dataReader As System.Data.IDataReader

		Public Property DataReader() As System.Data.IDataReader
			Get
				Return _dataReader
			End Get
			Set(ByVal Value As System.Data.IDataReader)
				_dataReader = Value
			End Set
		End Property

	End Class

End Namespace

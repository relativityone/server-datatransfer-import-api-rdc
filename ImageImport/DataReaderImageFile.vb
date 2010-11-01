Namespace kCura.WinEDDS.ImportExtension

	Public Class DataTableImageFile
		Inherits kCura.WinEDDS.ImageLoadFile
		Private _dataTable As System.Data.DataTable
		Public Property DataTable() As System.Data.DataTable
			Get
				Return _dataTable
			End Get
			Set(ByVal Value As System.Data.DataTable)
				_dataTable = Value
			End Set
		End Property

	End Class

End Namespace

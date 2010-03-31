Namespace kCura.Relativity.DataReaderClient
	Public Class Status

#Region " Private Variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver

		Private _Success As Boolean
		Private _Message As String
		Private _TotalRecordProcessed As Int32

#End Region

#Region " Public Properties "

		Public Property Message() As String
			Get
				Return _Message
			End Get
			Set(ByVal Value As String)
				_Message = Value
			End Set
		End Property

#End Region

#Region " Constructors "

		Public Sub New()

		End Sub

		Public Sub New(ByVal messageString As String)
			Message = messageString
		End Sub

#End Region
	End Class


End Namespace
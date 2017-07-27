Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Represents a status message provided to an OnMessage event.
	''' </summary>
	Public Class Status

#Region " Private Variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver

		Private _Success As Boolean
		Private _Message As String
		Private _TotalRecordProcessed As Int32

#End Region

#Region " Public Properties "

		''' <summary>
		''' Gets or sets the status message.
		''' </summary>
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

		''' <summary>
		''' Creates an empty Status without a message.
		''' </summary>
		Public Sub New()

		End Sub

		''' <summary>
		''' Creates a Status with the given message.
		''' </summary>
		''' <param name="messageString">The message provided to the Status.</param>
		Public Sub New(ByVal messageString As String)
			Message = messageString
		End Sub

#End Region
	End Class


End Namespace
Namespace kCura.Windows.Process
	Public MustInherit Class ProcessBase

		Private _processObserver As ProcessObserver
		Private _processController As Controller
		Private _processID As Guid

		Public Sub StartProcess()
			Try
				Me.Execute()
				_processObserver.RaiseProcessCompleteEvent()
			Catch ex As Exception
				_processObserver.RaiseFatalExceptionEvent(ex)
			End Try
		End Sub

		Protected MustOverride Sub Execute()

		Public ReadOnly Property ProcessObserver() As kCura.Windows.Process.ProcessObserver
			Get
				Return _processObserver
			End Get
		End Property

		Public ReadOnly Property ProcessController() As kCura.Windows.Process.Controller
			Get
				Return _processController
			End Get
		End Property

		Public Sub New()
			_processObserver = New kCura.Windows.Process.ProcessObserver
			_processController = New kCura.Windows.Process.Controller
		End Sub

		Public Property ProcessID() As Guid
			Get
				Return _processID
			End Get
			Set(ByVal value As Guid)
				_processID = value
			End Set
		End Property

	End Class
End Namespace
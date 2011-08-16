Namespace kCura.Windows.Process
	Public MustInherit Class ProcessBase
		Implements IRunable

		Private _processObserver As ProcessObserver
		Private _processController As Controller
		Private _processID As Guid

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

		Protected Sub New()
			_processObserver = New kCura.Windows.Process.ProcessObserver
			_processController = New kCura.Windows.Process.Controller
		End Sub

#Region " Implements IRunable "

		Public Property ProcessID() As Guid Implements IRunable.ProcessID
			Get
				Return _processID
			End Get
			Set(ByVal value As Guid)
				_processID = value
			End Set
		End Property

		Public Sub StartProcess() Implements IRunable.StartProcess
			Try
				Me.Execute()

				'TODO: PHIL LOOK HERE

				_processObserver.RaiseProcessCompleteEvent()
			Catch ex As Exception
				_processObserver.RaiseFatalExceptionEvent(ex)
			End Try
		End Sub

#End Region

	End Class
End Namespace
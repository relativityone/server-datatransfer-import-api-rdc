Namespace kCura.Windows.Process.Generic
	Public MustInherit Class ProcessBase(Of T)
		Implements IRunable

		Private _processObserver As ProcessObserver(Of T)
		Private _processController As Controller
		Private _processID As Guid

		Protected MustOverride Sub Execute()

		Public ReadOnly Property ProcessObserver() As kCura.Windows.Process.Generic.ProcessObserver(Of T)
			Get
				Return _processObserver
			End Get
		End Property

		Public ReadOnly Property ProcessController() As kCura.Windows.Process.Controller
			Get
				Return _processController
			End Get
		End Property

		Protected Sub New(ByVal getMessage As Func(Of T, String), ByVal getRecordInfo As Func(Of T, String))
			_processObserver = New kCura.Windows.Process.Generic.ProcessObserver(Of T)(getMessage, getRecordInfo)
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
				_processObserver.RaiseProcessCompleteEvent()
			Catch ex As Exception
				_processObserver.RaiseFatalExceptionEvent(ex)
			End Try
		End Sub

#End Region

	End Class
End Namespace
'Imports kCura.Utility.File.LineCounter
Namespace kCura.WinEDDS
	Public Class LoadFilePreProcessor
		Inherits LoadFileReader

		Public Enum EventType
			Begin
			Complete
			Progress
		End Enum

		Public Class EventArgs
			Private _newlinesRead As Int64
			Private _bytesRead As Int64
			Private _totalBytes As Int64
			Private _stepSize As Int64
			Private _type As EventType

			Public ReadOnly Property NewlinesRead() As Int64
				Get
					Return _newlinesRead
				End Get
			End Property

			Public ReadOnly Property BytesRead() As Int64
				Get
					Return _bytesRead
				End Get
			End Property

			Public ReadOnly Property TotalBytes() As Int64
				Get
					Return _totalBytes
				End Get
			End Property

			Public ReadOnly Property StepSize() As Int64
				Get
					Return _stepSize
				End Get
			End Property
			Public ReadOnly Property Type() As EventType
				Get
					Return _type
				End Get
			End Property

			Public Sub New(ByVal type As EventType, ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
				_newlinesRead = newlines
				_bytesRead = bytes
				_totalBytes = total
				_stepSize = [step]
				_type = type
			End Sub
		End Class

		Private WithEvents _haltListener As New HaltListener

		Public Sub New(ByVal args As LoadFile, ByVal trackErrorsAsFieldValues As Boolean)
			MyBase.New(args, trackErrorsAsFieldValues)
		End Sub

		Public Event OnEvent(ByVal e As kCura.WinEDDS.LoadFilePreProcessor.EventArgs)

		Public Sub CountLines()
			Me.ReadFile(_settings.FilePath)
		End Sub

		Protected Sub ProcessStart(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Begin, newlines, bytes, total, [step])
		End Sub

		Protected Sub ProcessProgress(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Progress, newlines, bytes, total, [step])
		End Sub

		Protected Sub ProcessComplete(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Complete, newlines, bytes, total, [step])
		End Sub

		Protected Sub RaiseOnEvent(ByVal type As EventType, ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseEvent OnEvent(New EventArgs(type, newlines, bytes, total, [step]))
		End Sub

		Public Sub StopCounting()
			_haltListener.Halt()
		End Sub

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Dim stepsize As Int32 = 1000 'TODO: Figure out WTF step is in original line counter

			Me.Reader = Nothing	'WRONG!!!!!

			While Not Me.HasReachedEOF
				If Me.CurrentLineNumber < 1000 Then	'TODO: make this a fucking constant :(
					'Do yer processing
				Else
					Me.AdvanceLine()
				End If

			End While
			Me.RaiseOnEvent(EventType.Complete, Me.CurrentLineNumber, Me.Reader.BaseStream.Position, Me.Reader.BaseStream.Length, stepsize)
		End Function


		Public Class HaltListener
			Public Event StopProcess()
			Public Sub Halt()
				RaiseEvent StopProcess()
			End Sub
		End Class

		Private Sub _haltListener_StopProcess() Handles _haltListener.StopProcess
			_continue = False
		End Sub


	End Class

End Namespace


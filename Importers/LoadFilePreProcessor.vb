Imports kCura.Utility.File.LineCounter
Namespace kCura.WinEDDS
	Public Class LoadFilePreProcessor
		Inherits LoadFileReader
		Public Sub New(ByVal args As LoadFile, ByVal trackErrorsAsFieldValues As Boolean)
			MyBase.New(args, trackErrorsAsFieldValues)
		End Sub
		Public Event OnEvent(ByVal e As kCura.Utility.File.LineCounter.EventArgs)


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
	End Class
End Namespace


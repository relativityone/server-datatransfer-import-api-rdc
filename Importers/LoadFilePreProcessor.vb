'Imports kCura.Utility.File.LineCounter
Namespace kCura.WinEDDS
	Public Class LoadFilePreProcessor
		Inherits LoadFileReader

		Public Enum EventType
			Begin
			Complete
			Progress
			Cancel
		End Enum

#Region "EventArgs"
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

#End Region

#Region "Members"
		Private WithEvents _haltListener As HaltListener
		Private _continue As Boolean
		Dim _folders As System.Collections.Specialized.HybridDictionary

#End Region

#Region "Event stuff"

		Public Event OnEvent(ByVal e As kCura.WinEDDS.LoadFilePreProcessor.EventArgs)

		Protected Sub ProcessStart(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Begin, newlines, bytes, total, [step])
		End Sub

		Protected Sub ProcessProgress(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Progress, newlines, bytes, total, [step])
		End Sub

		Protected Sub ProcessComplete(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Complete, newlines, bytes, total, [step])
		End Sub

		Protected Sub ProcessCancel(ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Cancel, newlines, bytes, total, [step])
		End Sub

		Protected Sub RaiseOnEvent(ByVal type As EventType, ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseEvent OnEvent(New EventArgs(type, newlines, bytes, total, [step]))
		End Sub

#End Region

		Public Sub New(ByVal args As LoadFile, ByVal trackErrorsAsFieldValues As Boolean)
			MyBase.New(args, trackErrorsAsFieldValues)
			_haltListener = New HaltListener
			_continue = True
			_folders = New System.Collections.Specialized.HybridDictionary
		End Sub

		Public Sub CountLines()
			If _settings.ForceFolderPreview AndAlso Not _settings.FolderStructureContainedInColumn Is Nothing AndAlso _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				Me.ReadFile(_settings.FilePath)
			Else
				Me.ReadFileSimple(_settings.FilePath)
			End If
		End Sub

		Public Function ReadFileSimple(ByVal path As String) As Object
			Me.EnsureReader()
			Dim fileSize As Int64 = Me.Reader.BaseStream.Length
			Dim stepSize As Int64 = GetStepSize(fileSize)
			Dim currentRun As Int64 = System.DateTime.Now.Ticks
			Dim lastRun As Int64 = currentRun

			Me.ProcessStart(Me.CurrentLineNumber, 0, filesize, stepsize)

			While Not Me.HasReachedEOF And _continue
				currentRun = System.DateTime.Now.Ticks
				If currentRun - lastRun > 10000000 Then
					lastRun = currentRun
					Me.ProcessProgress(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
				End If
				Me.AdvanceLine()
			End While
			Me.ProcessComplete(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
			Return Nothing
		End Function

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Me.EnsureReader()
			Dim fileSize As Int64 = Me.Reader.BaseStream.Length
			Dim stepSize As Int64 = GetStepSize(fileSize)
			'Dim bound As Char = Me.Bound
			'Dim delimeter As Char = Me.Delimiter
			Dim folderColumnIndex As Int32 = Me.GetColumnIndexFromString(_settings.FolderStructureContainedInColumn)
			Dim currentRun As Int64 = System.DateTime.Now.Ticks
			Dim lastRun As Int64 = currentRun
			Dim showedPopup As Boolean = False
			Dim popupRetVal As Int32 = -1
			Dim lineToParse As String()

			Me.ProcessStart(Me.CurrentLineNumber, 0, fileSize, stepSize)

			While Not Me.HasReachedEOF And _continue
				currentRun = System.DateTime.Now.Ticks
				If currentRun - lastRun > 10000000 Then
					lastRun = currentRun
					Me.ProcessProgress(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
				End If
				If Me.CurrentLineNumber < 1000 Then	'TODO: make this a fucking constant :(
					lineToParse = Me.GetLine()
					Me.AddFolder(lineToParse.GetValue(folderColumnIndex).ToString)
					If Me.CurrentLineNumber = 1000 AndAlso Not _settings.FolderStructureContainedInColumn Is Nothing Then
						showedPopup = True
						Dim msg As String = String.Format("The first {0} records of this load file will create {1} folders.  Would you like to continue?", Me.GetActualLineCount, Me.GetActualFolderCount)
						popupRetVal = MsgBox(msg, (MsgBoxStyle.OkCancel Or MsgBoxStyle.ApplicationModal))
						If Not popupRetVal = 1 Then Me.ProcessCancel(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
					End If
				Else
					Me.AdvanceLine()
				End If
			End While

			If Not showedPopup Then
				Dim msg As String = String.Format("The first {0} records of this load file will create {1} folders.  Would you like to continue?", Me.GetActualLineCount, Me.GetActualFolderCount)
				popupRetVal = MsgBox(msg, (MsgBoxStyle.OkCancel Or MsgBoxStyle.ApplicationModal))

				If Not popupRetVal = 1 Then
					Me.ProcessCancel(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
				Else
					Me.ProcessComplete(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
				End If
				
			End If

			Return Nothing
		End Function

		Private Function GetActualLineCount() As Int32
			If _settings.FirstLineContainsHeaders Then
				Return Me.CurrentLineNumber - 1
			Else
				Return Me.CurrentLineNumber
			End If
		End Function

		Private Function GetActualFolderCount() As Int32
			If _settings.FirstLineContainsHeaders Then
				Return _folders.Count - 1
			End If
			Return _folders.Count
		End Function

		Private Function GetColumnIndexFromString(ByVal pathColumn As String) As Int32
			Dim retVal As Int32 = -1
			If Not pathColumn Is Nothing Then
				Dim openParenIndex As Int32 = pathColumn.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = pathColumn.LastIndexOf(")"c)
				retVal = Int32.Parse(pathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			End If
			Return retVal
		End Function

		Private Sub AddFolder(ByVal folderpath As String)
			If folderpath <> "" AndAlso folderpath <> "\" Then
				If folderpath.LastIndexOf("\"c) < 1 Then
					If Not _folders.Contains(folderpath) Then _folders.Add(folderpath, "")
				Else
					If Not _folders.Contains(folderpath) Then _folders.Add(folderpath, "")
					AddFolder(folderpath.Substring(0, folderpath.LastIndexOf("\"c)))
				End If
			End If

		End Sub

		Private Function GetStepSize(ByVal value As Int64) As Int64
			Return CType(value / 100, Int64)
		End Function


#Region "HaltListener"
		Public Class HaltListener
			Public Event StopProcess()
			Public Sub Halt()
				RaiseEvent StopProcess()
			End Sub
		End Class

#End Region

		Public Sub StopCounting()
			_haltListener.Halt()
		End Sub

		Private Sub _haltListener_StopProcess() Handles _haltListener.StopProcess
			_continue = False
		End Sub

	End Class

End Namespace


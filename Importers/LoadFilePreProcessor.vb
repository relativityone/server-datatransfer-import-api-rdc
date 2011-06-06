Imports System.Collections.Generic
Imports System.Linq

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
		Dim _choicesTable As New Dictionary(Of Int32, Dictionary(Of String, Boolean))	 'Dictionary to track the choice values created per column index
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

		Private Function NeedToCheckFolders() As Boolean
			Return (_settings.ForceFolderPreview AndAlso _settings.CreateFolderStructure AndAlso Not _settings.FolderStructureContainedInColumn Is Nothing AndAlso _artifactTypeID = Relativity.ArtifactType.Document AndAlso _settings.OverwriteDestination = "None")
		End Function

		Private Function NeedToCheckChoices() As Boolean
			Return Me._fieldMap.ToArray() _
			 .Where(Function(item) item.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.Code Or item.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.MultiCode) _
			 .Count() > 0	 'Look for any choice fields in the _fieldMap
		End Function

		Public Sub CountLines()
			If NeedToCheckFolders() OrElse NeedToCheckChoices() Then
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

			Me.ProcessStart(Me.CurrentLineNumber, 0, fileSize, stepSize)

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
			Dim folderColumnIndex As Int32 = Me.GetColumnIndexFromString(_settings.FolderStructureContainedInColumn)
			Dim choiceColumnIndexList As New System.Collections.Generic.List(Of Int32)
			Dim currentRun As Int64 = System.DateTime.Now.Ticks
			Dim lastRun As Int64 = currentRun
			Dim showedPopup As Boolean = False
			Dim popupRetVal As Int32 = -1
			Dim lineToParse As String()
			Dim checkFolders As Boolean = NeedToCheckFolders()
			Dim checkChoices As Boolean = NeedToCheckChoices()
			Dim onFirstLine As Boolean = True

			'Inits
			Me.ProcessStart(Me.CurrentLineNumber, 0, fileSize, stepSize)

			'Prepare the choice count columns
			_choicesTable.Clear()
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap.ToArray() _
				.Where(Function(mitem) mitem.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.Code Or mitem.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.MultiCode)

				_choicesTable(item.NativeFileColumnIndex) = New Dictionary(Of String, Boolean)
			Next

			'Parse up to the first X lines in the file and track the folders and choices that will be created
			While Not Me.HasReachedEOF And Me.GetActualLineCount < kCura.WinEDDS.Config.PREVIEW_THRESHOLD And _continue
				'Report progress
				currentRun = System.DateTime.Now.Ticks
				If currentRun - lastRun > 10000000 Then
					lastRun = currentRun
					Me.ProcessProgress(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
				End If

				'Skip first line if needed
				If _settings.FirstLineContainsHeaders And onFirstLine Then
					AdvanceLine()
					onFirstLine = False
					Continue While
				End If

				'Parse each line and track folders and choices.
				lineToParse = Me.GetLine()
				If checkFolders Then AddFolder(lineToParse.GetValue(folderColumnIndex).ToString)
				If checkChoices Then
					For Each choiceColumnIdx As Int32 In _choicesTable.Keys
						Dim choiceVal As String = lineToParse.GetValue(choiceColumnIdx).ToString
						'Choices can be imported as comma-delimited strings for multi-choices, need to look at each choice
						For Each choiceSet As String In choiceVal.Split(New Char() {";"c})
							For Each choiceItem In choiceSet.Split(New Char() {"\"c})
								_choicesTable(choiceColumnIdx)(choiceItem) = True
							Next
						Next
					Next
				End If

			End While

			'Determine choice threshold
			Dim choiceCountThreshold As Int32
			If checkChoices Then
				choiceCountThreshold = _codeManager.GetChoiceLimitForUI()
			End If

			'Display choice and folder warning to the user
			If _continue Then
				Dim reportFolders As Boolean = checkFolders AndAlso Me.GetActualFolderCount > 0
				Dim reportChoices As Boolean = checkChoices AndAlso Me.GetMaxChoiceCount > choiceCountThreshold
				If reportFolders Or reportChoices Then
					Dim popupMsg As String = BuildImportWarningMessage(Me.GetActualLineCount, reportFolders, Me.GetActualFolderCount, reportChoices, Me.GetMaxChoiceCount)
					popupRetVal = MsgBox(popupMsg, (MsgBoxStyle.YesNo Or MsgBoxStyle.ApplicationModal), "Relativity Desktop Client")
					If popupRetVal <> MsgBoxResult.Yes Then
						Me.ProcessCancel(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
						Return Nothing
					End If
				End If
			End If

			'Cleanup
			Me.ProcessComplete(Me.CurrentLineNumber, Me.Reader.BaseStream.Position, fileSize, stepSize)
			Return Nothing
		End Function

		Private Function BuildImportWarningMessage(ByVal lineCount As Int32, ByVal reportFolders As Boolean, ByVal numOfFolders As Int32, ByVal reportChoices As Boolean, ByVal numOfChoices As Int32) As String
			Dim snippetList As New List(Of String)
			If reportFolders Then snippetList.Add(String.Format("{0} folders", numOfFolders))
			If reportChoices Then snippetList.Add(String.Format("{0} choices in a single field", numOfChoices))
			Dim countSegment As String = String.Join(" and ", snippetList.ToArray())
			Return String.Format("The first {0} records of this load file will create {1}.  Would you like to continue?", lineCount, countSegment)
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

		''' <summary>
		''' Returns maximum snumber of choices that will be created in any column of the import (based on the rows analyzed so far)
		''' </summary>
		Private Function GetMaxChoiceCount() As Int32
			Dim maxChoiceCount As Int32 = 0
			For Each choiceKey As Int32 In _choicesTable.Keys
				Dim currentChoiceCount As Int32 = _choicesTable(choiceKey).Keys.Count
				maxChoiceCount = Math.Max(maxChoiceCount, currentChoiceCount)
			Next
			Return maxChoiceCount
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


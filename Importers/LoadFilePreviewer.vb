'Imports kCura.EDDS.DynamicFields
Namespace kCura.WinEDDS
	Public Class LoadFilePreviewer
		Inherits kCura.WinEDDS.LoadFileBase

#Region "Members"
		Private _errorsOnly As Boolean
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _continue As Boolean = True
#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal timeZoneOffset As Int32, ByVal errorsOnly As Boolean, Optional ByVal processController As kCura.Windows.Process.Controller = Nothing)
			MyBase.New(args, timeZoneOffset)
			_errorsOnly = errorsOnly
			_processController = processController
		End Sub

#End Region

#Region "Event"

		Public Enum EventType
			Begin
			Complete
			Progress
		End Enum

		Public Class EventArgs
			Private _bytesRead As Int64
			Private _totalBytes As Int64
			Private _stepSize As Int64
			Private _type As EventType

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

			Public Sub New(ByVal type As EventType, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
				_bytesRead = bytes
				_totalBytes = total
				_stepSize = [step]
				_type = type
			End Sub
		End Class
		Public Event OnEvent(ByVal e As EventArgs)

		Private Sub ProcessStart(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Begin, bytes, total, [step])
		End Sub

		Private Sub ProcessProgress(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Progress, bytes, total, [step])
		End Sub

		Private Sub ProcessComplete(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Complete, bytes, total, [step])
		End Sub

		Private Sub RaiseOnEvent(ByVal type As EventType, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseEvent OnEvent(New EventArgs(type, bytes, total, [step]))
		End Sub
#End Region

		Private Function GetPosition() As Int32
			Return CType(Me.Reader.BaseStream.Position, Int32)
		End Function

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Dim earlyexit As Boolean = False
			Reader = New System.IO.StreamReader(path, System.Text.Encoding.Default)
			Dim filesize As Int64 = Reader.BaseStream.Length
			Dim stepsize As Int64 = CType(filesize / 100, Int64)
			ProcessStart(0, filesize, stepsize)
			Dim fieldArrays As New System.Collections.ArrayList
			If _firstLineContainsColumnNames Then
				_columnHeaders = GetLine
				If _uploadFiles Then
					Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
					Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
					_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				End If
				'_filePathColumnIndex = Array.IndexOf(_columnHeaders, _filePathColumn)
			Else
				If _uploadFiles Then
					_filePathColumnIndex = Int32.Parse(_filePathcolumn.Replace("Column", "").Replace("(", "").Replace(")", "").Trim) - 1
				End If
			End If
			Dim i As Int32 = 0
			While Not HasReachedEOF AndAlso _continue
				If fieldArrays.Count < 1000 Then
					Dim x As DocumentField() = CheckLine(getline)
					If Not x Is Nothing Then fieldArrays.Add(x)
					i += 1
					If i Mod 100 = 0 Then ProcessProgress(GetPosition, filesize, stepsize)
				Else
					earlyexit = True
					Exit While
				End If
			End While
			If earlyexit Then
				ProcessComplete(-1, filesize, -1)
			Else
				ProcessComplete(filesize, filesize, stepsize)
			End If
			Return fieldArrays
		End Function
		Private Function CheckLine(ByVal values As String()) As DocumentField()
			Dim mapItem As LoadFileFieldMap.LoadFileFieldMapItem
			Dim lineContainsErrors As Boolean = False
			Dim retval As New ArrayList
			Dim valToParse As String = ""
			For Each mapItem In _fieldMap
				If mapItem.NativeFileColumnIndex > -1 AndAlso Not mapItem.DocumentField Is Nothing Then
					Try
						valToParse = values(mapItem.NativeFileColumnIndex)
					Catch ex As System.Exception
						valToParse = ""
					End Try
					Dim docfield As New DocumentField(mapItem.DocumentField)
					lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docfield, valToParse, mapItem.NativeFileColumnIndex)
					retval.Add(docfield)
				End If
			Next
			If _uploadFiles Then
				Dim filePath As String = values(_filePathColumnIndex)
				Dim docfield As New DocumentField("Native File", -1, -1, -1, NullableInt32.Null, NullableInt32.Null)
				If filePath = "" Then
					docfield.Value = "No File Specified."
				ElseIf Not System.IO.File.Exists(filePath) Then
					docfield.Value = String.Format("File '{0}' does not exist", filePath)
					lineContainsErrors = True
				Else
					docfield.Value = filePath
				End If
				retval.Add(docfield)
			End If
			If _errorsOnly Then
				If lineContainsErrors Then
					Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
				Else
					Return Nothing
				End If
			Else
				Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
			End If
		End Function

		'Private Function CheckLine(ByVal values As String()) As DocumentField()
		'	Dim docFields(_docFields.Length - 1) As DocumentField
		'	Dim docField As DocumentField
		'	Dim i As Int32 = 0
		'	Dim lineContainsErrors As Boolean = False
		'	'Dim fieldIDs(_docFields.Length - 1) As Int32
		'	'Dim fieldValues(_docFields.Length - 1) As String
		'	For Each docField In _docfields
		'		docField.Value = ""
		'	Next
		'	For Each docField In _docFields
		'		lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docField, values(i), i)
		'		i += 1
		'		If values.Length - 1 < i Then
		'			Exit For
		'		End If
		'	Next
		'	For i = 0 To _docFields.Length - 1
		'		docFields(i) = New DocumentField(_docFields(i))
		'		docFields(i).Value = _docFields(i).Value
		'	Next
		'	If _errorsOnly Then
		'		If lineContainsErrors Then
		'			Return docFields
		'		Else
		'			Return Nothing
		'		End If
		'	Else
		'		Return docFields
		'	End If
		'End Function

		Private Function SetFieldValueOrErrorMessage(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32) As Boolean
			Try
				SetFieldValue(field, value, column, True)
				Return False
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				field.Value = ex.Message
				Return True
			End Try
		End Function

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub
	End Class
End Namespace


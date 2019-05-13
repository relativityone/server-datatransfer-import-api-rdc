Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Process

Namespace kCura.WinEDDS
	Public Class OpticonFileReader
		Inherits DelimitedFileImporter2
		Implements Api.IImageReader
		Private _settings As ImageLoadFile
		Public ReadOnly Property Settings() As ImageLoadFile
			Get
				Return _settings
			End Get
		End Property


		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal context As ProcessContext, ByVal processID As Guid, ByVal doRetryLogic As Boolean)
			MyBase.New(","c, doRetryLogic)
			_settings = args
		End Sub

		Public Sub AdvanceRecord() Implements Api.IImageReader.AdvanceRecord
			Me.AdvanceLine()
		End Sub

		Public Overloads Sub Close() Implements Api.IImageReader.Close
			MyBase.Close()
		End Sub

		Public ReadOnly Property CurrentRecordNumber() As Integer Implements Api.IImageReader.CurrentRecordNumber
			Get
				Return MyBase.CurrentLineNumber
			End Get
		End Property

		Private Enum Columns
			BatesNumber = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

		Public Function GetImageRecord() As Api.ImageRecord Implements Api.IImageReader.GetImageRecord
			Dim val As String() = Me.GetLine
			If val.Length < 4 Then Throw New InvalidLineFormatException(Me.CurrentLineNumber, val.Length)
			Dim retval As New Api.ImageRecord
			retval.BatesNumber = val(Columns.BatesNumber)
			retval.FileLocation = val(Columns.FileLocation)
			retval.IsNewDoc = val(Columns.MultiPageIndicator).ToLower = "y"
			Return retval
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements Api.IImageReader.HasMoreRecords
			Get
				Return Not Me.HasReachedEOF
			End Get
		End Property

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Throw New MethodAccessException("Unsupported Operation")
		End Function

		Public Function CountRecords() As Long Implements Api.IImageReader.CountRecords
			Return Global.Relativity.DataExchange.Io.FileSystem.Instance.File.CountLinesInFile(Me.Settings.FileName)
		End Function

		Public Sub Cancel() Implements Api.IImageReader.Cancel
			Me.Context.RetryOptions = RetryOptions.None
		End Sub

#Region " Exceptions - Fatal "

		''' <summary>
		''' The exception thrown when an opticon file contains an invalid line entry.
		''' </summary>
		<Serializable>
		Public Class InvalidLineFormatException
			Inherits System.Exception

			''' <summary>
			''' Initializes a new instance of the <see cref="InvalidLineFormatException"/> class.
			''' </summary>
			''' <param name="lineNumber">
			''' The line number where the error occurred.
			''' </param>
			''' <param name="numberOfColumns">
			''' The actual number of columns found within the line.
			''' </param>
			Public Sub New(ByVal lineNumber As Int32, ByVal numberOfColumns As Int32)
				MyBase.New(String.Format("Invalid opticon file line {0}.  There must be at least 4 columns per line in an opticon file, there are {1} in the current line", lineNumber, numberOfColumns))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

#End Region

		Public Sub Initialize() Implements Api.IImageReader.Initialize
			Me.Reader = New System.IO.StreamReader(_settings.FileName, System.Text.Encoding.Default, True)
			Me.Rewind()
		End Sub
	End Class
End Namespace


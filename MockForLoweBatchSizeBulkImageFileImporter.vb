Imports System.IO
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports System.Text

Namespace kCura.WinEDDS.NUnit

Public Class MockForLoweBatchSizeBulkImageFileImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private Property WillThrowException As Boolean
		Public _outPutFromStringWriter As New StringBuilder()

		Protected Overrides ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return True
			End Get
		End Property

		Public PauseCalled As Int32 = 0
		Private _inputForStringReader As String
		Private _importBatchSize As Int32

		Public Sub New(ByVal args As ImageLoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal throwsException As Boolean, ByVal bulkManager As kCura.WinEDDS.Service.BulkImportManager)
			MyBase.new(0, args, processController, processID, doRetryLogic, False)
			Me.WillThrowException = throwsException
			_bulkImportManager = bulkManager
			Me.ImportBatchSize = 500
			Me.ImportBatchVolume = 1000000
		End Sub

		Public Sub New(ByVal importBatchSize As Int32, ByVal inputForStringReader As String, ByVal args As ImageLoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal throwsException As Boolean, ByVal bulkManager As kCura.WinEDDS.Service.BulkImportManager)
			Me.New(args, processController, processID, doRetryLogic, throwsException, bulkManager)
			_inputForStringReader = inputForStringReader
			_importBatchSize = importBatchSize
		End Sub

		Public Sub MockLowerBatchSizeAndRetry(ByVal numberOfRecords As Int32)
			MyBase.LowerBatchSizeAndRetry("", "", numberOfRecords)
		End Sub

		Protected Overrides Function CreateStreamWriter(ByVal tmpLocation As String) As System.IO.TextWriter
			Return New StringWriter(_outPutFromStringWriter)
		End Function

		Protected Overrides Function CreateStreamReader(ByVal outputPath As String) As System.IO.TextReader
			Return New StringReader(_inputForStringReader)
		End Function

		Protected Overrides Property ImportBatchSize As Int32
			Get
				Return _importBatchSize
			End Get
			Set(ByVal value As Int32)

			End Set
		End Property

		Protected Overrides Function DoLogicAndPushImageBatch(ByVal totalRecords As Integer, ByVal recordsProcessed As Integer, ByVal bulkLocation As String, ByVal dataGridLocation As String, ByRef charactersSuccessfullyProcessed As Long, ByVal i As Integer, ByVal charactersProcessed As Long) As Integer
			Return 100
		End Function

		Public Function TryBulkImport(ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType) As MassImportResults
			Return RunBulkImport(overwrite, False)
		End Function

		Protected Overrides Sub InitializeDTOs(ByVal args As ImageLoadFile)
			_keyFieldDto = New kCura.EDDS.WebAPI.FieldManagerBase.Field
			_keyFieldDto.ArtifactID = 0
		End Sub

		Protected Overrides Sub InitializeUploaders(ByVal args As ImageLoadFile)
			'do nothing
		End Sub

		'Protected Overrides Sub InitializeManagers(ByVal args As ImageLoadFile)
		'	'do nothing
		'End Sub

		Protected Overrides Sub RaiseWarningAndPause(ByVal ex As System.Exception, ByVal timeoutSeconds As Integer)
			PauseCalled += 1
		End Sub

		Public Property MinimumBatch As Integer
			Get
				Return MyBase.MinimumBatchSize
			End Get
			Set(ByVal value As Integer)
				MinimumBatchSize = value
			End Set
		End Property

		Protected Overrides ReadOnly Property NumberOfRetries As Int32
			Get
				Return 3
			End Get
		End Property

	End Class
End NameSpace
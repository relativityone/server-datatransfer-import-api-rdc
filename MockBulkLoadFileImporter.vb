Imports System.Threading
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.WinEDDS.TApi
Imports Relativity.Logging

Namespace kCura.WinEDDS.NUnit
	''' <summary>
	''' Inheriting the BulkLoadFileImporter to override its BulkImportManager property (the part we want to throw timeout exceptions)
	''' </summary>
	Public Class MockBulkLoadFileImporter
		Inherits kCura.WinEDDS.BulkLoadFileImporter

		Private Property WillThrowException As Boolean

		Public Property FieldMap As LoadFileFieldMap
			Get
				Return MyBase._fieldMap
			End Get
			Set(ByVal value As LoadFileFieldMap)
				MyBase._fieldMap = value
			End Set
		End Property

		Public Property BatchSize As Int32
			Get
				Return Me.ImportBatchSize
			End Get
			Set(ByVal value As Int32)
				Me.ImportBatchSize = value
			End Set
		End Property

		Protected Overrides ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return True
			End Get
		End Property

		Public PauseCalled As Int32 = 0

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByRef ioReporterInstance As IIoReporter, ByRef logger As ILog, 
					   ByVal timeZoneOffset As Int32, ByVal autoDetect As Boolean, ByVal initializeUploaders As Boolean, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal bulkLoadFileFieldDelimiter As String, ByVal throwsException As Boolean, ByVal bulkManager As kCura.WinEDDS.Service.BulkImportManager,  ByRef tokenSource As CancellationTokenSource)
			MyBase.new(args, processController, ioReporterInstance, logger, timeZoneOffset, autoDetect, initializeUploaders, processID, doRetryLogic, bulkLoadFileFieldDelimiter, False, tokenSource)
			Me.WillThrowException = throwsException
			_bulkImportManager = bulkManager
			Me.ImportBatchSize = 500
			Me.ImportBatchVolume = 1000000
		End Sub

		Public Function CleanFolderPath(ByVal path As String) As String
			Return MyBase.CleanDestinationFolderPath(path)
		End Function

		Public Function ConvertOverlayBehaviorEnum(ByVal inputOverlayType As LoadFile.FieldOverlayBehavior?) As kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior
			Return MyBase.GetMassImportOverlayBehavior(inputOverlayType)
		End Function

		Public Function TryBulkImport(ByVal settings As NativeLoadInfo) As MassImportResults
			Return BulkImport(settings, True)
		End Function

		Protected Overrides ReadOnly Property BulkImportManager As kCura.WinEDDS.Service.BulkImportManager
			Get
				If _bulkImportManager Is Nothing Then _bulkImportManager = New MockBulkImportManagerWebExceptions(Me.WillThrowException)
				Return _bulkImportManager
			End Get
		End Property

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

		Protected Overrides ReadOnly Property ParentArtifactTypeID As Integer
			Get
				Return 4000
			End Get
		End Property

		Protected Overrides ReadOnly Property NumberOfRetries As Int32
			Get
				Return 3
			End Get
		End Property

	End Class
End NameSpace
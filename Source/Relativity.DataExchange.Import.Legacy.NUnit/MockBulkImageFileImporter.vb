' -----------------------------------------------------------------------------------------------------
' <copyright file="MockBulkImageFileImporter.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports System.Threading

Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.EDDS.WebAPI.DocumentManagerBase
Imports kCura.WinEDDS
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Process

Imports Relativity.Logging

Namespace Relativity.DataExchange.Import.NUnit

	''' <summary>
	''' Inheriting the BulkLoadFileImporter to override its BulkImportManager property (the part we want to throw timeout exceptions)
	''' </summary>
	Public Class MockBulkImageFileImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private Property WillThrowException As Boolean
		Public Property BatchSize As Int32
			Get
				Return Me.ImportBatchSize
			End Get
			Set(ByVal value As Int32)
				ImportBatchSize = value
			End Set
		End Property

		Protected Overrides ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return True
			End Get
		End Property

		Public PauseCalled As Int32 = 0

		Public Sub New(ByVal args As ImageLoadFile, ByVal processController As ProcessContext, ByVal ioReporterInstance As IIoReporter, ByVal logger As ILog, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal throwsException As Boolean, ByVal bulkManager As kCura.WinEDDS.Service.BulkImportManager, ByVal tokenSource As CancellationTokenSource)
			MyBase.New(0, args, processController, ioReporterInstance, logger, processID, doRetryLogic, tokenSource, AddressOf GetCorrelationIdFunc)
			Me.WillThrowException = throwsException
			_bulkImportManager = bulkManager
			Me.ImportBatchSize = 500
			Me.ImportBatchVolume = 1000000
		End Sub

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

		Private Shared Function GetCorrelationIdFunc() As String
			Return NameOf(MockBulkImageFileImporter)
		End Function
	End Class
End Namespace
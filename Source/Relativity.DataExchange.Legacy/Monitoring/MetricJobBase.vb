﻿Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer

Namespace Monitoring
	Public MustInherit Class MetricJobBase
		Inherits MetricBase
		
		''' <summary>
		''' Gets or sets job type - Import or Export.
		''' </summary>
		''' <returns>Job type</returns>
		Public Property TransferDirection As TelemetryConstants.TransferDirection
			Get
				Return GetValueOrDefault(Of TelemetryConstants.TransferDirection)(TelemetryConstants.KeyName.TRANSFER_DIRECTION)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.TRANSFER_DIRECTION) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets transfer mode - <see cref="TapiClient"/>
		''' </summary>
		''' <returns>Transfer mode</returns>
		Public Property TransferMode As TapiClient
			Get
				Return GetValueOrDefault(Of TapiClient)(TelemetryConstants.KeyName.TRANSFER_MODE)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.TRANSFER_MODE) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets name of application that executes a job.
		''' </summary>
		''' <returns>Application name</returns>
		Public Property ApplicationName As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.APPLICATION_NAME)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.APPLICATION_NAME) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets value indicating whether old export was used to perform job
		''' </summary>
		''' <returns>True if old export was used, False otherwise</returns>
		Public Property UseOldExport As Boolean
			Get
				Return GetValueOrDefault(Of Boolean)(TelemetryConstants.KeyName.USE_OLD_EXPORT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.USE_OLD_EXPORT) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets type of imported objects.
		''' </summary>
		''' <returns>Bulk import objects type.</returns>
		Public Property ImportObjectType As TelemetryConstants.ImportObjectType
			Get
				Return GetValueOrDefault (Of TelemetryConstants.ImportObjectType)(TelemetryConstants.KeyName.IMPORT_OBJECT_TYPE)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.IMPORT_OBJECT_TYPE) = Value
			End Set
		End Property
	End Class
End NameSpace
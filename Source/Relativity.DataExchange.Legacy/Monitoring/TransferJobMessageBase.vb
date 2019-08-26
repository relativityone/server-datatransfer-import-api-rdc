﻿Imports System.Collections.Generic
Imports Monitoring
Imports Relativity.DataExchange
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS.Monitoring
	Public MustInherit class TransferJobMessageBase
		Implements IMessage, IMetricMetadata

		Private Const JobTypeKeyName As String = "JobType"
		Private Const TransferModeKeyName As String = "TransferMode"
        Private Const ApplicationNameKeyName As String = "ApplicationName"

		Public Sub New()
			CustomData = New Dictionary(Of String, Object)
		End Sub

        Public MustOverride ReadOnly Property BucketName As String
        
		Public Property JobType As String
			Get
				Return GetValueOrDefault (Of String)(JobTypeKeyName)
			End Get
			Set
				CustomData.Item(JobTypeKeyName) = Value
			End Set
		End Property

		Public Property TransferMode As String
			Get
				Return GetValueOrDefault (Of String)(TransferModeKeyName)
			End Get
			Set
				CustomData.Item(TransferModeKeyName) = Value
			End Set
		End Property

        Public Property ApplicationName As String
            Get
                Return GetValueOrDefault (Of String)(ApplicationNameKeyName)
            End Get
            Set(value As String)
                CustomData.Item(ApplicationNameKeyName) = value
            End Set
        End Property

		Public Property CorrelationID As String Implements IMetricMetadata.CorrelationID
		Public Property CustomData As Dictionary(Of String, Object) Implements IMetricMetadata.CustomData
		Public Property WorkspaceID As Integer Implements IMetricMetadata.WorkspaceID
		Public Property UnitOfMeasure As String Implements IMetricMetadata.UnitOfMeasure

	end class
End NameSpace
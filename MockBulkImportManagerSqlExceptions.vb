Imports kCura.EDDS.WebAPI.BulkImportManagerBase

Namespace kCura.WinEDDS.NUnit
	''' <summary>
	''' Mock BulkImportManager for testing when SQL throws timeout Exceptions
	''' </summary>
	''' <remarks></remarks>
	Public Class MockBulkImportManagerSqlExceptions
		Inherits kCura.WinEDDS.Service.BulkImportManager

		Public Property ErrorMessage As kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException
		Private Property AlwaysThrow As Boolean
		Private Property BatchSizeThrow As Boolean
		Public Property MaxBatch As Int32

		Public Sub New(ByVal throwsException As Boolean)
			MyBase.new(Nothing, Nothing)
			If throwsException Then
				AlwaysThrow = True
				Me.ErrorMessage = New kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException(New SoapExceptionDetail())
			Else
				AlwaysThrow = False
				Me.ErrorMessage = Nothing
			End If
			MaxBatch = 100000	'just something large
		End Sub

		Public Sub New(ByVal batchSizeLimit As Int32)
			MyBase.new(Nothing, Nothing)
			Me.ErrorMessage = New kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException(New SoapExceptionDetail())
			AlwaysThrow = False
			BatchSizeThrow = True
			Me.MaxBatch = batchSizeLimit
		End Sub

#Region " Throw if given an excpetion "

		Protected Overrides Function InvokeBulkImportImage(ByVal appID As Integer, ByVal settings As ImageLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow OrElse BatchSizeThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean, ByVal rootFolderID As Int32) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow OrElse settings.Range IsNot Nothing Then Throw Me.ErrorMessage
			If settings.Range IsNot Nothing AndAlso settings.Range.Count > MaxBatch Then Throw Me.ErrorMessage
			If BatchSizeThrow AndAlso settings.Range Is Nothing Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow Then Throw Me.ErrorMessage
			If settings.Range IsNot Nothing AndAlso settings.Range.Count > MaxBatch Then Throw Me.ErrorMessage
			If BatchSizeThrow AndAlso settings.Range Is Nothing Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal settings As ImageLoadInfo, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow OrElse BatchSizeThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

#End Region

	End Class
End NameSpace
Namespace kCura.WinEDDS.NUnit
	''' <summary>
	''' Mock BulkImportManager for testing when the webservice throws timeout Exceptions
	''' </summary>
	''' <remarks></remarks>
	Public Class MockBulkImportManagerWebExceptions
		Inherits kCura.WinEDDS.Service.BulkImportManager

		Public Property ErrorMessage As System.Exception
		Private Property AlwaysThrow As Boolean

		Public Sub New(ByVal throwsException As Boolean)
			MyBase.new(Nothing, Nothing)
			If throwsException Then
				AlwaysThrow = True
				Me.ErrorMessage = New System.Net.WebException("The operation has timed out.")
			Else
				AlwaysThrow = False
				Me.ErrorMessage = Nothing
			End If
		End Sub

#Region " Throw if given an excpetion "

		Protected Overrides Function InvokeBulkImportImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean, ByVal rootFolderID As Int32) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

		Protected Overrides Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If AlwaysThrow Then Throw Me.ErrorMessage
			Return retval
		End Function

#End Region

	End Class
End NameSpace
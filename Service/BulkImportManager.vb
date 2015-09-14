Namespace kCura.WinEDDS.Service
	Public Class BulkImportManager
		Inherits kCura.EDDS.WebAPI.BulkImportManagerBase.BulkImportManager

#Region "Constructors"

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}BulkImportManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

		Private Sub CheckResultsForException(ByVal results As EDDS.WebAPI.BulkImportManagerBase.MassImportResults)
			If results.ExceptionDetail IsNot Nothing Then
				If results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("Timeout expired") Then
					Throw New BulkImportSqlTimeoutException(results.ExceptionDetail)
				ElseIf results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("##InsufficientPermissionsForImportException##") Then
					Throw New InsufficientPermissionsForImportException(results.ExceptionDetail)
				Else
					Throw New BulkImportSqlException(results.ExceptionDetail)
				End If
			End If
		End Sub

		Private Sub UnpackException(ByVal ex As System.Exception)
			Dim soapEx As System.Web.Services.Protocols.SoapException = TryCast(ex, System.Web.Services.Protocols.SoapException)
			If soapEx Is Nothing Then Return
			Dim permissionException As System.Exception = Nothing
			Try
				If soapEx.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "Relativity.Core.Exception.InsufficientAccessControlListPermissions" Then
					permissionException = New InsufficientPermissionsForImportException(soapEx.Detail.SelectNodes("ExceptionMessage")(0).InnerText)
				End If
			Catch
			End Try
			If Not permissionException Is Nothing Then Throw permissionException
		End Sub

		Private Function ExecuteImport(f As Func(Of kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Try
				Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = f()
				Me.CheckResultsForException(retval)
				Return retval
			Catch ex As Exception
				UnpackException(ex)
				Throw
			End Try
		End Function

		Private Function GenerateErrorFileKey(f As Func(Of kCura.EDDS.WebAPI.BulkImportManagerBase.ErrorFileKey)) As Relativity.MassImport.ErrorFileKey
			Dim retval As New Relativity.MassImport.ErrorFileKey
			With f()
				retval.LogKey = .LogKey
				retval.OpticonKey = .OpticonKey
			End With
			Return retval
		End Function

#Region " Shadow Methods "

		Public Shadows Function BulkImportImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Of kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)(Function() ExecuteImport(Function() Me.InvokeBulkImportImage(appID, settings, inRepository)))
		End Function

		Public Shadows Function BulkImportProductionImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Of kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)(Function() ExecuteImport(Function() Me.InvokeBulkImportProductionImage(appID, settings, productionKeyFieldArtifactID, inRepository)))
		End Function

		Public Shadows Function BulkImportNative(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Of kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)(Function() ExecuteImport(Function() Me.InvokeBulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)))
		End Function

		Public Shadows Function BulkImportObjects(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Of kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)(Function() ExecuteImport(Function() Me.InvokeBulkImportObjects(appID, settings, inRepository)))
		End Function

		Public Shadows Function GenerateImageErrorFiles(ByVal appID As Int32, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldId As Int32) As Relativity.MassImport.ErrorFileKey
			Return RetryOnReLoginException(Of Relativity.MassImport.ErrorFileKey)(Function() GenerateErrorFileKey(Function() MyBase.GenerateImageErrorFiles(appID, importKey, writeHeader, keyFieldId)))
		End Function

		Public Shadows Function GenerateNonImageErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As Relativity.MassImport.ErrorFileKey
			Return RetryOnReLoginException(Of Relativity.MassImport.ErrorFileKey)(Function() GenerateErrorFileKey(Function() MyBase.GenerateNonImageErrorFiles(appID, runID, artifactTypeID, writeHeader, keyFieldID)))
		End Function

		Public Shadows Function NativeRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
			Return RetryOnReLoginException(Of Boolean)(Function() MyBase.NativeRunHasErrors(appID, runId))
		End Function

		Public Shadows Function ImageRunHasErrors(ByVal appID As Int32, ByVal runId As String) As Boolean
			Return RetryOnReLoginException(Of Boolean)(Function() MyBase.ImageRunHasErrors(appID, runId))
		End Function

		Public Shadows Function DisposeTempTables(ByVal appID As Int32, ByVal runId As String) As Object
			Return RetryOnReLoginException(Of Object)(Function() MyBase.DisposeTempTables(appID, runId))
		End Function

		Public Shadows Function HasImportPermissions(ByVal appID As Integer) As Boolean
			Return RetryOnReLoginException(Of Boolean)(Function() MyBase.HasImportPermissions(appID))
		End Function

#End Region

#Region " Webservice Wrapper Methods "

		Protected Overridable Function InvokeBulkImportImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportImage(appID, settings, inRepository)
		End Function

		Protected Overridable Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportProductionImage(appID, settings, productionKeyFieldArtifactID, inRepository)
		End Function

		Protected Overridable Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)
		End Function

		Protected Overridable Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportObjects(appID, settings, inRepository)
		End Function

#End Region

		Public Class BulkImportSqlException
			Inherits System.Exception

			Private Property DetailedException As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception.ExceptionMessage)
				Me.DetailedException = exception
			End Sub

			Public Overrides Function ToString() As String
				Return DetailedException.ExceptionFullText
			End Function
		End Class

		Public Class BulkImportSqlTimeoutException
			Inherits BulkImportSqlException
			Private _details As String()
			Public ReadOnly Property Details As String()
				Get
					Return _details
				End Get
			End Property
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception)
				_details = exception.Details
			End Sub
		End Class

		Public Class InsufficientPermissionsForImportException
			Inherits System.Exception

			Public Sub New(message As String)
				MyBase.New(message)
			End Sub
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception.ExceptionMessage.Replace("##InsufficientPermissionsForImportException##", ""))
			End Sub
		End Class

	End Class


End Namespace

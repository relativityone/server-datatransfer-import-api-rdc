Imports Relativity.Import.Export

Namespace kCura.WinEDDS.Service
	Public Class BulkImportManager
		Inherits kCura.EDDS.WebAPI.BulkImportManagerBase.BulkImportManager

#Region "Constructors"

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}BulkImportManager.asmx", AppSettings.Instance.WebApiServiceUrl)
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

		Private Function GenerateErrorFileKey(f As Func(Of kCura.EDDS.WebAPI.BulkImportManagerBase.ErrorFileKey)) As Global.Relativity.MassImport.ErrorFileKey
			Dim retval As New Global.Relativity.MassImport.ErrorFileKey
			With f()
				retval.LogKey = .LogKey
				retval.OpticonKey = .OpticonKey
			End With
			Return retval
		End Function

#Region " Shadow Methods "

		Public Shadows Function BulkImportImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Function() ExecuteImport(Function() Me.InvokeBulkImportImage(appID, settings, inRepository)))
		End Function

		Public Shadows Function BulkImportProductionImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Function() ExecuteImport(Function() Me.InvokeBulkImportProductionImage(appID, settings, productionKeyFieldArtifactID, inRepository)))
		End Function

		Public Shadows Function BulkImportNative(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Function() ExecuteImport(Function() Me.InvokeBulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)))
		End Function

		Public Shadows Function BulkImportObjects(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return RetryOnReLoginException(Function() ExecuteImport(Function() Me.InvokeBulkImportObjects(appID, settings, inRepository)))
		End Function

		Public Shadows Function GenerateImageErrorFiles(ByVal appID As Int32, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldId As Int32) As Global.Relativity.MassImport.ErrorFileKey
			Return RetryOnReLoginException(Function() GenerateErrorFileKey(Function() MyBase.GenerateImageErrorFiles(appID, importKey, writeHeader, keyFieldId)))
		End Function

		Public Shadows Function GenerateNonImageErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As Global.Relativity.MassImport.ErrorFileKey
			Return RetryOnReLoginException(Function() GenerateErrorFileKey(Function() MyBase.GenerateNonImageErrorFiles(appID, runID, artifactTypeID, writeHeader, keyFieldID)))
		End Function

		Public Shadows Function NativeRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
			Return RetryOnReLoginException(Function() MyBase.NativeRunHasErrors(appID, runId))
		End Function

		Public Shadows Function ImageRunHasErrors(ByVal appID As Int32, ByVal runId As String) As Boolean
			Return RetryOnReLoginException(Function() MyBase.ImageRunHasErrors(appID, runId))
		End Function

		Public Shadows Function DisposeTempTables(ByVal appID As Int32, ByVal runId As String) As Object
			Return RetryOnReLoginException(Function() MyBase.DisposeTempTables(appID, runId))
		End Function

		Public Shadows Function HasImportPermissions(ByVal appID As Integer) As Boolean
			Return RetryOnReLoginException(Function() MyBase.HasImportPermissions(appID))
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

#Region " Exceptions "

		''' <summary>
		''' The exception thrown when a SQL exception is thrown when performing a bulk import service call.
		''' </summary>
		<Serializable>
		Public Class BulkImportSqlException
			Inherits System.Exception

			Public Property DetailedException As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			Public Sub New(ByVal message As String)
				MyBase.New(message)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			''' <param name="innerException">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal message As String, ByVal innerException As Exception)
				MyBase.New(message, innerException)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlException"/> class.
			''' </summary>
			''' <param name="exception">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception.ExceptionMessage)
				Me.DetailedException = exception
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
				Me.DetailedException = TryCast(info.GetValue("DetailedException", GetType(EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)), EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
				info.AddValue("DetailedException", Me.DetailedException)
				MyBase.GetObjectData(info, context)
			End Sub

			Public Overrides Function ToString() As String
				If Me.DetailedException Is Nothing
					Return String.Empty
				End If

				Return Me.DetailedException.ExceptionFullText
			End Function
		End Class

		''' <summary>
		''' The exception thrown when a SQL timeout exception is thrown when performing a bulk import service call.
		''' </summary>
		<Serializable>
		Public Class BulkImportSqlTimeoutException
			Inherits BulkImportSqlException

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlTimeoutException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlTimeoutException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			Public Sub New(ByVal message As String)
				MyBase.New(message)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlTimeoutException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			''' <param name="innerException">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal message As String, ByVal innerException As Exception)
				MyBase.New(message, innerException)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="BulkImportSqlTimeoutException"/> class.
			''' </summary>
			''' <param name="exception">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception)
				Me.Details = exception.Details
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
				Me.Details = TryCast(info.GetValue("Details", GetType(String())), String())
			End Sub

			''' <summary>
			''' Gets the list of details.
			''' </summary>
			''' <value>
			''' The details.
			''' </value>
			Public ReadOnly Property Details As String()

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
				info.AddValue("Details", Me.Details)
				MyBase.GetObjectData(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a SQL timeout exception is thrown when performing a bulk import service call.
		''' </summary>
		<Serializable>
		Public Class InsufficientPermissionsForImportException
			Inherits System.Exception

			''' <summary>
			''' Initializes a new instance of the <see cref="InsufficientPermissionsForImportException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="InsufficientPermissionsForImportException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			Public Sub New(ByVal message As String)
				MyBase.New(message)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="InsufficientPermissionsForImportException"/> class.
			''' </summary>
			''' <param name="exception">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(ExtractErrorMessage(exception))
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="InsufficientPermissionsForImportException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			''' <param name="innerException">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(ByVal message As String, ByVal innerException As Exception)
				MyBase.New(message, innerException)
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub

			Private Shared Function ExtractErrorMessage(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail) As String
				If (exception Is Nothing) Then
					Return Nothing
				End If

				If (String.IsNullOrEmpty(exception.ExceptionMessage)) Then
					Return exception.ExceptionMessage
				End If

				Return exception.ExceptionMessage.Replace("##InsufficientPermissionsForImportException##", "")
			End Function
		End Class

#End Region

	End Class
End Namespace

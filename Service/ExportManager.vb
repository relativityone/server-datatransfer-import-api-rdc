﻿Imports kCura.WinEDDS.Service.Export

Namespace kCura.WinEDDS.Service
	Public Class ExportManager
		Inherits kCura.EDDS.WebAPI.ExportManagerBase.ExportManager
		Implements Export.IExportManager
		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}ExportManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Private Function MakeCallAttemptReLogin(Of T)(f As Func(Of T)) As T
			Return RetryOnReLoginException(Of T)(
				Function()
					Try
						Return f()
					Catch ex As System.Exception
						UnpackHandledException(ex)
						Throw
					End Try
				End Function)
		End Function

		Public Function RetrieveResultsBlockForProductionStartingFromIndex(appID As Integer, runId As Guid, artifactTypeID As Integer, avfIds As Integer(), chunkSize As Integer, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Integer(), productionId As Integer, index As Integer) As Object() Implements IExportManager.RetrieveResultsBlockForProductionStartingFromIndex
			Dim retval As Object() = MakeCallAttemptReLogin(Function() MyBase.RetrieveResultsBlockForProductionStartingFromIndex(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds, productionId, index))
			RehydrateStrings(retval)
			Return retval
		End Function

		Public Shadows Function InitializeFolderExport(ByVal appID As Int32, ByVal viewArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal includeSubFolders As Boolean, ByVal avfIds As Int32(), ByVal startAtRecord As Int32, ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements Export.IExportManager.InitializeFolderExport
			Return MakeCallAttemptReLogin(Function() MyBase.InitializeFolderExport(appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID))
		End Function

		Public Shadows Function InitializeProductionExport(ByVal appID As Int32, ByVal productionArtifactID As Int32, ByVal avfIds As Int32(), ByVal startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements Export.IExportManager.InitializeProductionExport
			Return MakeCallAttemptReLogin(Function() MyBase.InitializeProductionExport(appID, productionArtifactID, avfIds, startAtRecord))
		End Function

		Public Shadows Function InitializeSearchExport(ByVal appID As Int32, ByVal searchArtifactID As Int32, ByVal avfIds As Int32(), ByVal startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements Export.IExportManager.InitializeSearchExport
			Return MakeCallAttemptReLogin(Function() MyBase.InitializeSearchExport(appID, searchArtifactID, avfIds, startAtRecord))
		End Function

		Public Shadows Function RetrieveResultsBlock(ByVal appID As Int32, ByVal runId As Guid, ByVal artifactTypeID As Int32, ByVal avfIds As Int32(), ByVal chunkSize As Int32, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char, ByVal textPrecedenceAvfIds As Int32()) As Object() Implements Export.IExportManager.RetrieveResultsBlock
			Dim retval As Object() = MakeCallAttemptReLogin(Function() MyBase.RetrieveResultsBlock(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds))
			RehydrateStrings(retval)
			Return retval
		End Function

		Public Function RetrieveResultsBlockStartingFromIndex(appID As Integer, runId As Guid, artifactTypeID As Integer, avfIds As Integer(), chunkSize As Integer, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Integer(), index As Integer) As Object() Implements IExportManager.RetrieveResultsBlockStartingFromIndex
			Dim retval As Object() = MakeCallAttemptReLogin(Function() MyBase.RetrieveResultsBlockStartingFromIndex(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds, index))
			RehydrateStrings(retval)
			Return retval
		End Function

		Public Shadows Function RetrieveResultsBlockForProduction(ByVal appID As Int32, ByVal runId As Guid, ByVal artifactTypeID As Int32, ByVal avfIds As Int32(), ByVal chunkSize As Int32, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char, ByVal textPrecedenceAvfIds As Int32(), ByVal productionId As Int32) As Object() Implements Export.IExportManager.RetrieveResultsBlockForProduction
			Dim retval As Object() = MakeCallAttemptReLogin(Function() MyBase.RetrieveResultsBlockForProduction(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds, productionId))
			RehydrateStrings(retval)
			Return retval
		End Function

		Private Sub RehydrateStrings(toScrub As Object())
			If Not toScrub Is Nothing Then
				For Each row As Object() In toScrub
					If row Is Nothing Then
						Throw New System.Exception("Invalid (null) row retrieved from server")
					End If
					For i As Int32 = 0 To row.Length - 1
						If TypeOf row(i) Is Byte() Then row(i) = System.Text.Encoding.Unicode.GetString(DirectCast(row(i), Byte()))
					Next
				Next
			End If

		End Sub

		Public Shadows Function HasExportPermissions(appID As Int32) As Boolean Implements Export.IExportManager.HasExportPermissions
			Return MakeCallAttemptReLogin(Function() MyBase.HasExportPermissions(appID))
		End Function

		Private Sub UnpackHandledException(ByVal ex As System.Exception)
			Dim soapEx As System.Web.Services.Protocols.SoapException = TryCast(ex, System.Web.Services.Protocols.SoapException)
			If soapEx Is Nothing Then Return
			Dim x As System.Exception = Nothing
			Try
				If soapEx.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "Relativity.Core.Exception.InsufficientAccessControlListPermissions" Then
					x = New InsufficientPermissionsForExportException(soapEx.Detail.SelectNodes("ExceptionMessage")(0).InnerText, ex)
				End If
			Catch
			End Try
			If Not x Is Nothing Then Throw x
		End Sub

		Public Class InsufficientPermissionsForExportException
			Inherits System.Exception

			Public Sub New(message As String)
				MyBase.New(message)
			End Sub
			Public Sub New(message As String, ex As System.Exception)
				MyBase.New(message, ex)
			End Sub
		End Class

	End Class
End Namespace


Imports System.Web

Namespace kCura.WinEDDS.Service
	Public Class CodeManager
		Inherits kCura.EDDS.WebAPI.CodeManagerBase.CodeManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}CodeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function CreateNewCodeDTOProxy(ByVal codeTypeID As Int32, ByVal name As String, ByVal order As Int32, ByVal caseSystemID As Int32) As kCura.EDDS.WebAPI.CodeManagerBase.Code
			Dim code As New kCura.EDDS.WebAPI.CodeManagerBase.Code
			code.CodeType = codeTypeID
			code.IsActive = True
			code.Name = name
			code.Order = order
			code.Keywords = String.Empty
			code.Notes = String.Empty
			code.ParentArtifactID = New Nullable(Of Int32)(caseSystemID)
			code.ContainerID = New Nullable(Of Int32)(caseSystemID)
			code.RelativityApplications = New Int32() {}
			Return code
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveCodesAndTypesForCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveCodesAndTypesForCase(caseContextArtifactID))
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal code As kCura.EDDS.WebAPI.CodeManagerBase.Code) As Object
			Dim tries As Int32 = 0
			Dim encode As Boolean = True
			Dim name As String = code.Name
			Dim nameEncoded As String = HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(code.Name))
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If encode Then
						code.Name = nameEncoded
						Return MyBase.CreateEncoded(caseContextArtifactID, code)
					Else
						code.Name = name
						Return MyBase.Create(caseContextArtifactID, code)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.Message.IndexOf("Server did not recognize the value of HTTP Header") <> -1 AndAlso encode Then
						'DAM202: This error only occurs if the RDC is newer than the RelativityWebAPI and the new method (CreateEncoded) does not exist in the WebAPI.
						'We do not want to use a relogin try when this error occurs.
						tries -= 1
						encode = False
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function ReadID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal codeTypeID As Int32, ByVal name As String) As Int32
			Dim tries As Int32 = 0
			Dim encode As Boolean = True
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If encode Then
						Return MyBase.ReadIDEncoded(caseContextArtifactID, parentArtifactID, codeTypeID, HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(name)))
					Else
						Return MyBase.ReadID(caseContextArtifactID, parentArtifactID, codeTypeID, name)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.Message.IndexOf("Server did not recognize the value of HTTP Header") <> -1 AndAlso encode Then
						tries -= 1
						encode = False
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

#End Region

		Public Shadows Function GetInitialChunk(ByVal caseContextArtifactID As Integer, ByVal codeTypeID As Integer) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.GetInitialChunk(caseContextArtifactID, codeTypeID))
		End Function

		Public Shadows Function GetLastChunk(ByVal caseContextArtifactID As Integer, ByVal codeTypeID As Integer, ByVal lastCodeID As Integer) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.GetLastChunk(caseContextArtifactID, codeTypeID, lastCodeID))
		End Function

		Public Function RetrieveAllCodesOfType(ByVal caseContextArtifactID As Int32, ByVal codeTypeID As Int32) As Relativity.ChoiceInfo()
			Dim dt As System.Data.DataTable
			Dim retval As New System.Collections.ArrayList
			Dim lastcodeId As Int32 = -1
			Do
				If lastcodeId = -1 Then
					dt = Me.GetInitialChunk(caseContextArtifactID, codeTypeID).Tables(0)
				Else
					dt = Me.GetLastChunk(caseContextArtifactID, codeTypeID, lastcodeId).Tables(0)
				End If
				For Each row As System.Data.DataRow In dt.Rows
					retval.Add(New Relativity.ChoiceInfo(row))
				Next
				If retval.Count > 0 Then lastcodeId = DirectCast(retval(retval.Count - 1), Relativity.ChoiceInfo).ArtifactID
			Loop Until dt Is Nothing OrElse dt.Rows.Count = 0
			Return DirectCast(retval.ToArray(GetType(Relativity.ChoiceInfo)), Relativity.ChoiceInfo())
		End Function

		Public Shadows Function RetrieveCodeByNameAndTypeID(ByVal caseContextArtifactID As Int32, ByVal codeTypeID As Int32, ByVal name As String) As Relativity.ChoiceInfo
			Dim tries As Int32 = 0
			Dim encode As Boolean = True
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If encode Then
						Return Convert(MyBase.RetrieveCodeByNameAndTypeIDEncoded(caseContextArtifactID, codeTypeID, HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(name))))
					Else
						Return Convert(MyBase.RetrieveCodeByNameAndTypeID(caseContextArtifactID, codeTypeID, name))
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.Message.IndexOf("Server did not recognize the value of HTTP Header") <> -1 AndAlso encode Then
						tries -= 1
						encode = False
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetChoiceLimitForUI() As Int32
			Return RetryOnReLoginException(Function() MyBase.GetChoiceLimitForUI())
		End Function

		Public Shared Function Convert(ByVal webserviceChoiceInfo As kCura.EDDS.WebAPI.CodeManagerBase.ChoiceInfo) As Relativity.ChoiceInfo
			If webserviceChoiceInfo Is Nothing Then Return Nothing
			Dim retval As New Relativity.ChoiceInfo
			retval.ArtifactID = webserviceChoiceInfo.ArtifactID
			retval.CodeTypeID = webserviceChoiceInfo.CodeTypeID
			retval.Name = webserviceChoiceInfo.Name
			retval.Order = webserviceChoiceInfo.Order
			retval.ParentArtifactID = webserviceChoiceInfo.ParentArtifactID
			Return retval
		End Function

	End Class
End Namespace
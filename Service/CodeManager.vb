Namespace kCura.WinEDDS.Service
	Public Class CodeManager
		Inherits kCura.EDDS.WebAPI.CodeManagerBase.CodeManager

		Private ReadOnly _serviceURLPageFormat As String

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webURL As String)
			MyBase.New()

			_serviceURLPageFormat = "{0}CodeManager.asmx"
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.ServiceURL = webURL
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

		Public Overridable Property ServiceURL As String
			Get
				Return Me.Url
			End Get
			Set(ByVal value As String)
				Me.Url = String.Format(_serviceURLPageFormat, value)
			End Set
		End Property

#Region " Shadow Functions "
		Public Shadows Function RetrieveCodesAndTypesForCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveCodesAndTypesForCase(caseContextArtifactID)
					Else
						'Return _codeManager.ExternalRetrieveCodesAndTypesForCase(caseContextArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal code As kCura.EDDS.WebAPI.CodeManagerBase.Code) As Object
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.Create(caseContextArtifactID, code)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function ReadID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal codeTypeID As Int32, ByVal name As String) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.ReadID(caseContextArtifactID, parentArtifactID, codeTypeID, name)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

#End Region

		Public Shadows Function GetInitialChunk(ByVal caseContextArtifactID As Integer, ByVal codeTypeID As Integer) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.GetInitialChunk(caseContextArtifactID, codeTypeID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetLastChunk(ByVal caseContextArtifactID As Integer, ByVal codeTypeID As Integer, ByVal lastCodeID As Integer) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.GetLastChunk(caseContextArtifactID, codeTypeID, lastCodeID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
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
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return Convert(MyBase.RetrieveCodeByNameAndTypeID(caseContextArtifactID, codeTypeID, name))
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetChoiceLimitForUI() As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.GetChoiceLimitForUI()
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return 0
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
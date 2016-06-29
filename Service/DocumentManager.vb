Imports kCura.Utility.NullableTypesHelper
Imports System.Runtime.Remoting.Lifetime
Namespace kCura.WinEDDS.Service
	Public Class DocumentManager
		Inherits kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}DocumentManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ReadFromDocumentArtifactID(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document = Me.Read(caseContextArtifactID, documentArtifactID, fieldArtifactIds)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				If field.FieldCategoryID = Relativity.FieldCategory.FullText Then
					field.Value = System.Text.Encoding.Default.GetString(DirectCast(field.Value, Byte()))
					Exit For
				End If
			Next
			Return doc
		End Function

#Region " Shadow Functions "
		Public Shadows Function GetAllDocumentsForCase(ByVal caseID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.GetAllDocumentsForCase(caseID))
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Return RetryOnReLoginException(Function() MyBase.Read(caseContextArtifactID, artifactID, fieldArtifactIds))
		End Function

		Public Shadows Function Update(ByVal caseContextArtifactID As Int32, ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Return RetryOnReLoginException(Function() MyBase.Update(caseContextArtifactID, document, files))
		End Function

		Public Shadows Function GetDocumentArtifactIDFromIdentifier(ByVal caseContextArtifactID As Int32, ByVal identifier As String, ByVal fieldDisplayName As String) As Int32
			Return RetryOnReLoginException(Function() MyBase.GetDocumentArtifactIDFromIdentifier(caseContextArtifactID, identifier, fieldDisplayName))
		End Function

		Public Shadows Function ReadFromIdentifier(ByVal caseContextArtifactID As Int32, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.ReadFromIdentifier(caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("ResetFieldIdCacheException") <> -1 Then

					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Sub ClearImagesFromDocument(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32)
			RetryOnReLoginException(Sub() MyBase.ClearImagesFromDocument(caseContextArtifactID, artifactID))
		End Sub

		Public Shadows Function RetrieveAllUnsupportedOiFileIds() As Int32()
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllUnsupportedOiFileIds())
		End Function

		Public Shadows Function GetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Int32) As String
			Return RetryOnReLoginException(Function() MyBase.GetDocumentDirectoryByCaseArtifactID(caseArtifactID))
		End Function

		Public Shadows Function GetPrintImageGuids(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal orderedProductionIDList As Int32()) As Guid()
			Return RetryOnReLoginException(Function() MyBase.GetPrintImageGuids(caseContextArtifactID, artifactID, orderedProductionIDList))
		End Function

		Public Shadows Function RetrieveDocumentCount(ByVal caseContextArtifactID As Int32) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.RetrieveDocumentCount(caseContextArtifactID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function RetrieveDocumentLimit(ByVal caseContextArtifactID As Int32) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.RetrieveDocumentLimit(caseContextArtifactID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function
#End Region

	End Class
End Namespace
Imports System.Net
Imports kCura.WinEDDS.Service

Namespace kCura.WinEDDS

	Public Class FieldProviderCache
		Implements IFieldProviderCache

		Private _fields As DocumentFieldCollection
		Private _fieldsNonFile As DocumentFieldCollection

		Private ReadOnly _cookieContainer As CookieContainer
		Private ReadOnly _credential As NetworkCredential
		Private ReadOnly _correlationIdFunc As Func(Of String)


		Public Sub New(credential As NetworkCredential, cookieContainer As CookieContainer, correlationIdFunc As Func(Of String))
			_cookieContainer = cookieContainer
			_credential = credential
			_correlationIdFunc = correlationIdFunc
		End Sub


		Public Sub ResetCache() Implements IFieldProviderCache.ResetCache
			_fields = Nothing
			_fieldsNonFile = Nothing
		End Sub

		Public ReadOnly Property CurrentFields(artifactTypeId As Int32, workspaceId As Integer,
												Optional ByVal refresh As Boolean = False) As DocumentFieldCollection Implements IFieldProviderCache.CurrentFields
			Get
				If _fields Is Nothing OrElse refresh Then
					_fields = GetFields(artifactTypeId, workspaceId, Function(field) True)
				End If
				Return _fields
			End Get
		End Property

		Public ReadOnly Property CurrentNonFileFields(artifactTypeId As Int32, workspaceId As Integer,
												Optional ByVal refresh As Boolean = False) As DocumentFieldCollection Implements IFieldProviderCache.CurrentNonFileFields
			Get
				If _fieldsNonFile Is Nothing OrElse refresh Then
					_fieldsNonFile = GetFields(artifactTypeId, workspaceId, Function(field) field.FieldTypeID <> 9)
				End If
				Return _fieldsNonFile
			End Get
		End Property

		Private Function GetFields(artifactTypeId As Int32, workspaceId As Integer, filterFun As Func(Of EDDS.WebAPI.DocumentManagerBase.Field, Boolean)) As DocumentFieldCollection

			Dim retFields As New DocumentFieldCollection

			Dim fieldManager As Service.Replacement.IFieldQuery = ManagerFactory.CreateFieldQuery(_credential, _cookieContainer, _correlationIdFunc)
			Dim fields() As EDDS.WebAPI.DocumentManagerBase.Field
			fields = fieldManager.RetrieveAllAsArray(workspaceId, artifactTypeId)
			Dim i As Int32
			For i = 0 To fields.Length - 1
				With fields(i)
					If filterFun(fields(i)) Then
						retFields.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .AssociativeArtifactTypeID, .UseUnicodeEncoding, .ImportBehavior, .EnableDataGrid))
					End If
				End With
			Next
			Return retFields
		End Function

	End Class

End Namespace

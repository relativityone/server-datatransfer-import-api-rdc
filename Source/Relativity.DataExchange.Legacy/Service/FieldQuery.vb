Imports System.Collections.Generic
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service
	Public Class FieldQuery
		Inherits kCura.EDDS.WebAPI.FieldQueryBase.FieldQuery
		Implements Replacement.IFieldQuery

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FieldQuery.asmx", AppSettings.Instance.WebApiServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function RetrieveAllAsArray(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, Optional ByVal includeUnmappable As Boolean = False) As kCura.EDDS.WebAPI.DocumentManagerBase.Field() Implements Replacement.IFieldQuery.RetrieveAllAsArray
			Dim dv As New SqlDataView(RetrieveAllMappable(caseContextArtifactID, artifactTypeID))
			Dim fields As New System.Collections.ArrayList
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim unmappableFields As New System.Collections.Specialized.StringCollection
			Dim unmappableFieldCategories As New System.Collections.ArrayList
			If Not includeUnmappable Then
				unmappableFieldCategories.AddRange(New FieldCategory() {FieldCategory.Reflected, FieldCategory.Batch, FieldCategory.MultiReflected, FieldCategory.FileInfo, FieldCategory.AutoCreate, FieldCategory.FileSize, FieldCategory.ProductionMarker, FieldCategory.MarkupSetMarker})
			End If
			Dim i As Int32
			For i = 0 To dv.Count - 1
				field = New kCura.EDDS.WebAPI.DocumentManagerBase.Field
				If Not ( _
				 unmappableFieldCategories.Contains(CType(dv(i)("FieldCategoryID"), FieldCategory)) _
				 OrElse _
				 unmappableFields.Contains(dv(i)("DisplayName").ToString) _
				) Then
					If Not (CType(dv(i)("FieldCategoryID"), FieldCategory) = FieldCategory.FullText AndAlso artifactTypeID <> ArtifactType.Document) Then
						Dim guidsString As String = dv(i)("ArtifactGuids").ToString()
						Dim guids As New List(Of Guid)
						If (Not String.IsNullOrEmpty(guidsString)) Then
							Dim guidStringArray As String() = guidsString.Split(CChar(";"))
							For Each guidString As String In guidStringArray
								guids.Add(New Guid(guidString.Trim()))
							Next
						End If
						With field
							.ArtifactID = CType(dv(i)("ArtifactID"), Int32)
							.ArtifactViewFieldID = CType(dv(i)("ArtifactViewFieldID"), Int32)
							.CodeTypeID = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("CodeTypeID"))
							.DisplayName = CType(dv(i)("DisplayName"), String)
							.FieldCategoryID = CType(dv(i)("FieldCategoryID"), Int32)
							.FieldCategory = CType(dv(i)("FieldCategoryID"), kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
							.FieldType = CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.FieldType), CType(dv(i)("FieldTypeID"), FieldType).ToString), kCura.EDDS.WebAPI.DocumentManagerBase.FieldType)
							.FieldTypeID = CInt(dv(i)("FieldTypeID"))
							.IsEditable = CType(dv(i)("IsEditable"), Boolean)
							.IsRequired = CType(dv(i)("IsRequired"), Boolean)
							.MaxLength = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("FieldLength"))
							.IsRemovable = CType(dv(i)("IsRemovable"), Boolean)
							.IsVisible = CType(dv(i)("IsVisible"), Boolean)
							.UseUnicodeEncoding = CType(dv(i)("UseUnicodeEncoding"), Boolean)
							.AllowHtml = CType(dv(i)("AllowHTML"), Boolean)
							.AssociativeArtifactTypeID = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("AssociativeArtifactTypeID"))
							.ImportBehavior = Me.ConvertImportBehaviorEnum(NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("ImportBehavior")))
							.EnableDataGrid = CBool(dv(i)("EnableDataGrid"))
							.Guids = guids.ToArray()
						End With
						If field.FieldType = EDDS.WebAPI.DocumentManagerBase.FieldType.Object OrElse field.FieldType = EDDS.WebAPI.DocumentManagerBase.FieldType.Objects OrElse field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.MultiReflected OrElse field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Reflected Then
							If field.AssociativeArtifactTypeID.HasValue AndAlso ArtifactTypeHelper.IsDynamic(field.AssociativeArtifactTypeID.Value) Then fields.Add(field)
						Else
							fields.Add(field)
						End If
					End If
				End If

			Next
			Return DirectCast(fields.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
		End Function

		Private Function ConvertImportBehaviorEnum(ByVal input As Int32?) As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice?
			If Not input.HasValue Then Return Nothing
			Dim ibc As ImportBehaviorChoice = CType(input, ImportBehaviorChoice)
			Return CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice), ibc.ToString), kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice)
		End Function

		Public Function RetrieveAllAsDocumentFieldCollection(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As DocumentFieldCollection Implements Replacement.IFieldQuery.RetrieveAllAsDocumentFieldCollection
			Dim retval As New DocumentFieldCollection
			For Each fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.RetrieveAllAsArray(caseContextArtifactID, artifactTypeID)
				With (fieldDTO)
					retval.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .AssociativeArtifactTypeID, .UseUnicodeEncoding, .ImportBehavior, .Guids, .EnableDataGrid))
				End With
			Next
			Return retval
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveDisplayFieldNameByFieldCategoryID(ByVal caseContextArtifactID As Int32, ByVal fieldCategoryID As Int32) As String
			Return RetryOnReLoginException(Function() MyBase.RetrieveDisplayFieldNameByFieldCategoryID(caseContextArtifactID, fieldCategoryID))
		End Function

		Public Shadows Function RetrieveAllMappable(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As System.Data.DataSet Implements Replacement.IFieldQuery.RetrieveAllMappable
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllMappable(caseContextArtifactID, artifactTypeID))
		End Function

		Public Shadows Function RetrieveAll(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveAll(caseContextArtifactID))
		End Function

		Public Shadows Function RetrievePotentialBeginBatesFields(ByVal caseContextArtifactID As Int32) As System.Data.DataSet Implements Replacement.IFieldQuery.RetrievePotentialBeginBatesFields
			Return RetryOnReLoginException(Function() MyBase.RetrievePotentialBeginBatesFields(caseContextArtifactID))
		End Function

		Public Shadows Function IsFieldIndexed(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As Boolean Implements Replacement.IFieldQuery.IsFieldIndexed
			Return RetryOnReLoginException(Function() MyBase.IsFieldIndexed(caseContextArtifactID, fieldArtifactID))
		End Function

#End Region

	End Class
End Namespace
Imports kCura.Utility.NullableTypesHelper
Namespace kCura.WinEDDS.Service
	Public Class FieldQuery
		Inherits kCura.EDDS.WebAPI.FieldQueryBase.FieldQuery

		'Private _fieldQuery As New kCura.EDDS.Service.DynamicFields.FieldQuery
		'Private _identity As kCura.EDDS.EDDSIdentity

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			'Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FieldQuery.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			'_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function RetrieveAllAsArray(ByVal caseContextArtifactID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Dim dv As New kCura.Data.DataView(RetrieveAllMappable(caseContextArtifactID))
			Dim fields As New System.Collections.ArrayList
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim unmappableFields As New System.Collections.Specialized.StringCollection
			unmappableFields.AddRange(New String() {"Has Annotations", "Has Images", "Has Native", "Redacted"})		' HACK: Ugly - need to make a new field category ID
			Dim i As Int32
			For i = 0 To dv.Count - 1
				field = New kCura.EDDS.WebAPI.DocumentManagerBase.Field
				If Not ( _
				 CType(dv(i)("FieldCategoryID"), kCura.DynamicFields.Types.FieldCategory) = DynamicFields.Types.FieldCategory.ProductionMarker _
				 OrElse _
				 unmappableFields.Contains(dv(i)("DisplayName").ToString) _
				) Then
					With field
						.ArtifactID = CType(dv(i)("ArtifactID"), Int32)
						.ArtifactViewFieldID = CType(dv(i)("ArtifactViewFieldID"), Int32)
						.CodeTypeID = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(dv(i)("CodeTypeID"))
						.DisplayName = CType(dv(i)("DisplayName"), String)
						.FieldCategoryID = CType(dv(i)("FieldCategoryID"), Int32)
						.FieldType = CType(dv(i)("FieldTypeID"), kCura.EDDS.WebAPI.DocumentManagerBase.FieldType)
						.FieldTypeID = CType(dv(i)("FieldTypeID"), kCura.EDDS.WebAPI.DocumentManagerBase.FieldType)
						.IsEditable = CType(dv(i)("IsEditable"), Boolean)
						.IsRequired = CType(dv(i)("IsRequired"), Boolean)
						.MaxLength = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(dv(i)("MaxLength"))
						.IsRemovable = CType(dv(i)("IsRemovable"), Boolean)
						.IsVisible = CType(dv(i)("IsVisible"), Boolean)
					End With
					fields.Add(field)
				End If

			Next
			Return DirectCast(fields.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveDisplayFieldNameByFieldCategoryID(ByVal caseContextArtifactID As Int32, ByVal fieldCategoryID As Int32) As String
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveDisplayFieldNameByFieldCategoryID(caseContextArtifactID, fieldCategoryID)
			Else
				'Return CType(_fieldQuery.RetrieveByFieldCategoryID(_identity, fieldCategoryID, contextArtifactID)("DisplayName"), String)
			End If
		End Function

		Public Shadows Function RetrieveAllMappable(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveAllMappable(caseContextArtifactID)
			Else
				'Return _fieldQuery.RetrieveAllMappable(_identity, caseID).ToDataSet
			End If
		End Function

		Public Shadows Function RetrieveAll(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveAll(caseContextArtifactID)
			Else
				'Return _fieldQuery.RetrieveAllWithSecurity(_identity, caseID).ToDataSet
			End If
		End Function
#End Region

	End Class
End Namespace
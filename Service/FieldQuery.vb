Imports kCura.Utility.NullableTypesHelper
Namespace kCura.WinEDDS.Service
	Public Class FieldQuery
		Inherits kCura.EDDS.WebAPI.FieldQueryBase.FieldQuery

		Private _fieldQuery As New kCura.EDDS.Service.DynamicFields.FieldQuery
		Private _identity As kCura.EDDS.EDDSIdentity

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FieldQuery.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function RetrieveAllAsArray(ByVal caseID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Dim dv As New kCura.Data.DataView(RetrieveAllMappable(caseID))
			Dim fields(dv.Count - 1) As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim i As Int32
			For i = 0 To fields.Length - 1
				field = New kCura.EDDS.WebAPI.DocumentManagerBase.Field
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
					.Removable = CType(dv(i)("Removable"), Boolean)
					.Visible = CType(dv(i)("Visible"), Boolean)
				End With
				fields(i) = field
			Next
			Return fields
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveDisplayFieldNameByFieldCategoryID(ByVal fieldCategoryID As Int32, ByVal contextArtifactID As Int32) As String
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveDisplayFieldNameByFieldCategoryID(fieldCategoryID, contextArtifactID)
			Else
				Return CType(_fieldQuery.RetrieveByFieldCategoryID(_identity, fieldCategoryID, contextArtifactID)("DisplayName"), String)
			End If
		End Function

		Public Shadows Function RetrieveAllMappable(ByVal caseID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveAllMappable(caseID)
			Else
				Return _fieldQuery.RetrieveAllMappable(_identity, caseID).ToDataSet
			End If
		End Function

		Public Shadows Function RetrieveAll(ByVal caseID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveAll(caseID)
			Else
				Return _fieldQuery.RetrieveAllWithSecurity(_identity, caseID).ToDataSet
			End If
		End Function
#End Region

	End Class
End Namespace
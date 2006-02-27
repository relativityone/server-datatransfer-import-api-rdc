Imports kCura.Utility.NullableTypesHelper
Namespace kCura.WinEDDS.Service
	Public Class FieldQuery
		Inherits kCura.EDDS.WebAPI.FieldQueryBase.FieldQuery

		Public Sub New(ByVal credentials As Net.NetworkCredential)
			MyBase.New()
			Me.Credentials = credentials
			Me.Url = String.Format("{0}FieldQuery.asmx", kCura.WinEDDS.Config.URI)
			Me.Timeout = Settings.DefaultTimeOut
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
					.CodeArtifactTypeID = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(dv(i)("CodeArtifactTypeID"))
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

	End Class
End Namespace
Namespace kCura.WinEDDS.Service
	Public Class ProductionManager
		Inherits kCura.EDDS.WebAPI.ProductionManagerBase.ProductionManager

		Private _productionManager As New kCura.EDDS.Service.ProductionManager
		Private _identity As kCura.EDDS.EDDSIdentity

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}ProductionManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Translations "
		Public Function DTOToWebAPIProduction(ByVal productionDTO As kCura.EDDS.DTO.Production) As kCura.EDDS.WebAPI.ProductionManagerBase.Production
			Dim production As New kCura.EDDS.WebAPI.ProductionManagerBase.Production

			production.AccessControlListID = productionDTO.AccessControlListID
			production.AccessControlListIsInherited = productionDTO.AccessControlListIsInherited
			production.ArtifactID = productionDTO.ArtifactID
			production.ArtifactTypeID = productionDTO.ArtifactTypeID
			production.ContainerID = productionDTO.ContainerID
			production.CreatedBy = productionDTO.CreatedBy
			production.CreatedOn = productionDTO.CreatedOn
			production.DeleteFlag = productionDTO.DeleteFlag
			production.Keywords = productionDTO.Keywords
			production.LastModifiedBy = productionDTO.LastModifiedBy
			production.LastModifiedOn = productionDTO.LastModifiedOn
			production.Notes = productionDTO.Notes
			production.ParentArtifactID = productionDTO.ParentArtifactID
			production.TextIdentifier = productionDTO.TextIdentifier
			production.BatesFormat = productionDTO.BatesFormat
			production.BatesPrefix = productionDTO.BatesPrefix
			production.BatesStartNumber = productionDTO.BatesStartNumber
			production.BatesSuffix = productionDTO.BatesSuffix
			production.BeginBatesFieldArtifactID = productionDTO.BeginBatesFieldArtifactID
			production.BurnAnnotations = productionDTO.BurnAnnotations
			production.CenterFooterFieldArtifactID = productionDTO.CenterFooterFieldArtifactID
			production.CenterFooterFreeText = productionDTO.CenterFooterFreeText
			production.CenterFooterTypeCodeArtifactID = productionDTO.CenterFooterTypeCodeArtifactID
			production.CenterHeaderFieldArtifactID = productionDTO.CenterHeaderFieldArtifactID
			production.CenterHeaderFreeText = productionDTO.CenterHeaderFreeText
			production.CenterHeaderTypeCodeArtifactID = productionDTO.CenterHeaderTypeCodeArtifactID
			production.DateProduced = productionDTO.DateProduced
			production.EndBatesFieldArtifactID = productionDTO.EndBatesFieldArtifactID
			production.FontSize = productionDTO.FontSize
			production.ImageShrinkPercent = productionDTO.ImageShrinkPercent
			production.LeftFooterFieldArtifactID = productionDTO.LeftFooterFieldArtifactID
			production.LeftFooterFreeText = productionDTO.LeftFooterFreeText
			production.LeftFooterTypeCodeArtifactID = productionDTO.LeftFooterTypeCodeArtifactID
			production.LeftHeaderFieldArtifactID = productionDTO.LeftHeaderFieldArtifactID
			production.LeftHeaderFreeText = productionDTO.LeftHeaderFreeText
			production.LeftHeaderTypeCodeArtifactID = productionDTO.LeftHeaderTypeCodeArtifactID
			production.Name = productionDTO.Name
			production.RightFooterFieldArtifactID = productionDTO.RightFooterFieldArtifactID
			production.RightFooterFreeText = productionDTO.RightFooterFreeText
			production.RightFooterTypeCodeArtifactID = production.RightFooterTypeCodeArtifactID
			production.RightHeaderFieldArtifactID = productionDTO.RightHeaderFieldArtifactID
			production.RightHeaderFreeText = productionDTO.RightHeaderFreeText
			production.RightHeaderTypeCodeArtifactID = productionDTO.RightHeaderTypeCodeArtifactID
			production.StatusCodeArtifactID = productionDTO.StatusCodeArtifactID
			production.SubdirectoryMaxFiles = productionDTO.SubdirectoryMaxFiles
			production.SubdirectoryPrefix = productionDTO.SubdirectoryPrefix
			production.SubdirectoryStartNumber = productionDTO.SubdirectoryStartNumber
			production.VolumeMaxSize = productionDTO.VolumeMaxSize
			production.VolumePrefix = productionDTO.VolumePrefix
			production.VolumeStartNumber = productionDTO.VolumeStartNumber
			Return production
		End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function RetrieveProducedByContextArtifactID(ByVal contextArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveProducedByContextArtifactID(contextArtifactID)
			Else
				Return _productionManager.ExternalRetrieveProducedByContextArtifactID(contextArtifactID, _identity)
			End If
		End Function

		Public Shadows Function Read(ByVal productionArtifactID As Int32) As kCura.EDDS.WebAPI.ProductionManagerBase.Production
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Read(productionArtifactID)
			Else
				Return Me.DTOToWebAPIProduction(_productionManager.Read(productionArtifactID, _identity))
			End If
		End Function

		Public Shadows Function RetrieveProducedWithSecurity(ByVal contextArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveProducedByContextArtifactID(contextArtifactID)
			Else
				Return _productionManager.ExternalRetrieveProducedWithSecurity(contextArtifactID, _identity)
			End If
		End Function
#End Region

	End Class
End Namespace
Imports System.Collections.Generic
Imports System.Linq
Imports AutoMapper
Imports Relativity.Applications.Serialization
Imports Relativity.Applications.Serialization.Elements
Imports kCura.EDDS.WebAPI

Namespace kCura.EDDS.WinForm.Data

	Public Class MappingCandidateFinder
		Implements IMappingCandidateFinder

		Private DOCUMENT_GUID As Guid = Guid.Parse("15C36703-74EA-4FF8-9DFB-AD30ECE7530D")

		Private _credentials As System.Net.ICredentials
		Private _cookieContainer As System.Net.CookieContainer
		Private _workspaceID As Int32

		Public Sub New(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal workspaceID As Int32)
			_credentials = credentials
			_cookieContainer = cookieContainer
			_workspaceID = workspaceID
		End Sub

		Public Sub PopulateMappingCandidates(ByVal appXml As System.Xml.XmlDocument, ByVal appMappingData As AppMappingData) Implements IMappingCandidateFinder.PopulateMappingCandidates
			Dim appElement = ApplicationElement.Deserialize(Of ApplicationElement)(appXml)
			Dim templateMgr As New Service.TemplateManager(_credentials, _cookieContainer)
			Dim fieldElements As kCura.EDDS.WebAPI.TemplateManagerBase.FieldElement()

			'Handle Document special case
			For Each obj In appElement.Objects
				If obj.Guid = New Guid() Then
					obj.Guid = DOCUMENT_GUID
				End If
			Next

			'Find out which objects are mapping candidates
			Dim objectGuidList = (From appObject In appElement.Objects Select appObject.Guid).ToArray()
			Dim objectsForMapping = templateMgr.GetAvailableObjectsForMapping(_workspaceID, objectGuidList).ToList()
			Dim objectGuidsAvailableForMapping = objectsForMapping.Select(Function(item) item.ObjectGuid).ToList()
			For Each appObjToCheck In appMappingData.AppObjects
				If objectGuidsAvailableForMapping.Contains(appObjToCheck.ObjectGuid) Then
					appObjToCheck.QualifiesForMapping = True
				End If
			Next

			'Build Guid Map
			Dim guidMap = SerializationHelper.GetArtifactTypeIDToGuidsMap(appXml)
			Dim apiGuidMap = New List(Of kCura.EDDS.WebAPI.TemplateManagerBase.ArtifactTypeIDToObjectGuids)
			AutoMapper.Mapper.CreateMap(Of Relativity.Applications.Serialization.ArtifactTypeIDToObjectGuids, kCura.EDDS.WebAPI.TemplateManagerBase.ArtifactTypeIDToObjectGuids)()
			For Each guidMapItem In guidMap
				'Convert guidMap to the web service proxy class
				Dim apuGuidItem = Mapper.Map(Of Relativity.Applications.Serialization.ArtifactTypeIDToObjectGuids, kCura.EDDS.WebAPI.TemplateManagerBase.ArtifactTypeIDToObjectGuids)(guidMapItem)
				'Handle Document special case
				If apuGuidItem.ObjectGuids.Count = 1 AndAlso apuGuidItem.ObjectGuids(0) = New Guid() Then
					apuGuidItem.ObjectGuids(0) = DOCUMENT_GUID
				End If
				apiGuidMap.Add(apuGuidItem)
			Next

			Dim allFieldGuids As System.Collections.Generic.List(Of System.Guid) = Relativity.Applications.Serialization.SerializationHelper.GetAllFieldGuids(appElement)


			'Populate mapping candidates
			For Each obj In appElement.Objects
				Dim objectGuidToCheck = obj.Guid
				Dim appObject = appMappingData.AppObjects.First(Function(item) item.ObjectGuid = objectGuidToCheck)
				If Not appObject.QualifiesForMapping Then
					Continue For
				End If
				Dim apiFieldElementList As New List(Of kCura.EDDS.WebAPI.TemplateManagerBase.FieldElement)
				Mapper.CreateMap(Of Elements.FieldElement, TemplateManagerBase.FieldElement)() _
				 .ForMember(Function(dest) dest.Codes, Sub(opt) opt.MapFrom(Function(srcField) MapCodeElementCollection(srcField)))
				Mapper.CreateMap(Of Relativity.Applications.Serialization.Elements.PaneElement, kCura.EDDS.WebAPI.TemplateManagerBase.PaneElement)()
				For Each field In obj.Fields
					Dim apiFieldElement = Mapper.Map(Of Relativity.Applications.Serialization.Elements.FieldElement, kCura.EDDS.WebAPI.TemplateManagerBase.FieldElement)(field)
					apiFieldElementList.Add(apiFieldElement)
				Next
				fieldElements = apiFieldElementList.ToArray()
				If Not fieldElements.Count() > 0 Then Continue For
				Dim objValResult = templateMgr.GetAvailableFieldsForMapping(_workspaceID, obj.Guid, fieldElements, apiGuidMap.ToArray())
				If objValResult.FieldToFieldsMaps IsNot Nothing Then
					For Each fieldMapResult In objValResult.FieldToFieldsMaps
						Dim appFieldGuid = fieldMapResult.FieldGuid
						Dim appField = appObject.AppFields.First(Function(item) item.FieldGuids.Contains(appFieldGuid))
						For Each candidate In fieldMapResult.Fields
							Dim targetField = appObject.FindExistingTargertField(candidate.ArtifactId)
							If targetField Is Nothing Then
								targetField = New TargetField()
								targetField.ArtifactID = candidate.ArtifactId
								If candidate.Guids IsNot Nothing Then
									targetField.FieldGuids.AddRange(candidate.Guids)
								End If
								targetField.MyName = candidate.DisplayName
							End If
							If allFieldGuids.Intersect(targetField.FieldGuids).Count() > 0 Then Continue For
							appField.MappingCandidates.Add(targetField)
						Next
					Next
				End If
			Next

			'Remove fields that have no mapping candidatesa
			Dim appObjsToRemove As New List(Of AppObject)
			For Each appObj In appMappingData.AppObjects
				Dim appObjToCheck = appObj
				Dim appFldsToRemove = New List(Of AppField)
				For Each appFld In appObj.AppFields
					If appFld.MappingCandidates.Count = 0 Then
						appFldsToRemove.Add(appFld)
					End If
				Next
				appFldsToRemove.ForEach(Sub(item) appObjToCheck.AppFields.Remove(item))
				If appObj.AppFields.Count = 0 Then
					appObjsToRemove.Add(appObj)
				End If
			Next
			appObjsToRemove.ForEach(Sub(item) appMappingData.AppObjects.Remove(item))

		End Sub

		Private Function MapCodeElementCollection(ByVal field As Elements.FieldElement) As kCura.EDDS.WebAPI.TemplateManagerBase.CodeElement()
			Dim resultList As New List(Of kCura.EDDS.WebAPI.TemplateManagerBase.CodeElement)
			Mapper.CreateMap(Of Relativity.Applications.Serialization.Elements.CodeElement, kCura.EDDS.WebAPI.TemplateManagerBase.CodeElement)()
			For Each code As CodeElement In field.Codes
				Dim apiCodeElement = Mapper.Map(Of Relativity.Applications.Serialization.Elements.CodeElement, kCura.EDDS.WebAPI.TemplateManagerBase.CodeElement)(code)
				resultList.Add(apiCodeElement)
			Next
			Return resultList.ToArray()
		End Function

	End Class

End Namespace
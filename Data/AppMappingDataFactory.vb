Imports System.Collections.Generic
Imports Relativity.Applications.Serialization.Elements

Namespace kCura.EDDS.WinForm.Data

	Public Class AppMappingDataFactory

		Public Function CreateMappingData(ByVal appElement As ApplicationElement) As AppMappingData
			Dim mapData As New AppMappingData

			For Each importObj In appElement.Objects
				Dim rand As New Random()
				Dim appObj As New AppObject() With {.ObjectName = importObj.Name, .ObjectGuid = importObj.Guid}

				'Handle Document object special-case
				If appObj.ObjectGuid = New Guid() Then
					appObj.ObjectGuid = Guid.Parse("15C36703-74EA-4FF8-9DFB-AD30ECE7530D")
				End If

				'Populate fields
				For Each importField In importObj.Fields
					Dim appField As New AppField With {.MyName = importField.DisplayName}
					appField.FieldGuids.Add(importField.Guid)	'TODO: Use guids?
					appObj.AppFields.Add(appField)
				Next
				mapData.AppObjects.Add(appObj)
			Next

			Return mapData
		End Function

	End Class

End Namespace
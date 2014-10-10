Imports System.Data
Imports System.Linq
Imports System.Collections.Generic
Imports System.ComponentModel
Imports Relativity.Applications.Serialization.Elements
Imports kCura.EDDS.WebAPI.TemplateManagerBase

Namespace kCura.EDDS.WinForm.Presentation.Controller

	Public Class FieldMapFourPickerController

		Public AppObjectsList As New BindingList(Of AppObject)
		Public AppFieldsList_NotMapped As New BindingList(Of AppField)
		Public AppFieldsList_Mapped As New BindingList(Of AppField)
		Public TargetFieldsList_NotMapped As New BindingList(Of TargetField)
		Public TargetFieldsList_Mapped As New BindingList(Of TargetField)

		Private _mappingData As AppMappingData
		Private _selectedObject As AppObject
		Private _selectedAppFieldForMapping As AppField
		Private _mappedByObjectTable As New Dictionary(Of AppObject, List(Of AppField))

		Public Sub New(ByVal mappingData As AppMappingData)
			_mappingData = mappingData
			For Each appObject In _mappingData.AppObjects.Where(Function(item) item.QualifiesForMapping)
				AppObjectsList.Add(appObject)
				_mappedByObjectTable(appObject) = New List(Of AppField)
			Next
		End Sub

		Public Function AreMappingsValid() As Boolean
			For Each appObject In _mappedByObjectTable.Keys
				For Each appField In appObject.AppFields
					'Is there an AppField in the MappedByObject list that hasn't been paired with a TargetField yet?
					If appField.MappedTargetField.IsEmpty And _mappedByObjectTable(appObject).Contains(appField) Then
						Return False
					End If
				Next
			Next
			Return True	'No errors, all is good
		End Function

		Public Sub SelectObject(ByVal appObject As AppObject)
			'Checks
			If Not _mappingData.AppObjects.Contains(appObject) Then
				Throw New ArgumentException("Supplied AppObject is not contained by the mapping data")
			End If

			If appObject.QualifiesForMapping = False Then
				Throw New ArgumentException("Supplied AppObject does not qualify for mapping")
			End If

			'Update data
			_selectedAppFieldForMapping = Nothing
			_selectedObject = appObject

			'Update lists
			AppFieldsList_NotMapped.Clear()
			Dim appField_NotMapped_Query = _selectedObject.AppFields.Except(_mappedByObjectTable(_selectedObject))
			appField_NotMapped_Query.ToList().ForEach(Sub(item) AppFieldsList_NotMapped.Add(item))

			AppFieldsList_Mapped.Clear()
			TargetFieldsList_Mapped.Clear()
			For Each appField In _mappedByObjectTable(_selectedObject)
				AppFieldsList_Mapped.Add(appField)
				TargetFieldsList_Mapped.Add(appField.MappedTargetField)
			Next

			TargetFieldsList_NotMapped.Clear()

		End Sub

		Public Sub SelectAppFieldForMapping(ByVal appField As AppField)
			'Checks
			CheckThatAnAppObjectHasBeenSelected()
			CheckThatAppFieldBelongsToCurrentlySelectedObject(appField)

			'Update data
			_selectedAppFieldForMapping = appField

			'Update lists
			TargetFieldsList_NotMapped.Clear()
			Dim targetField_NotMapped_Query = appField.MappingCandidates.Except(GetTargetFieldsAlreadyMapped(_selectedObject))
			targetField_NotMapped_Query.ToList().ForEach(Sub(item) TargetFieldsList_NotMapped.Add(item))

		End Sub

		Public Sub DeselectAppFieldForMapping()
			'Update data
			_selectedAppFieldForMapping = Nothing

			'Update lists
			TargetFieldsList_NotMapped.Clear()

		End Sub

		Public Sub MapAppField(ByVal appField As AppField)
			'Checks
			CheckThatAnAppObjectHasBeenSelected()
			CheckThatAppFieldBelongsToCurrentlySelectedObject(appField)
			CheckThatAppFieldNotAlreadyInMappedList(appField)

			'Update data
			_mappedByObjectTable(_selectedObject).Add(appField)

			'Update lists
			AppFieldsList_NotMapped.Remove(appField)
			AppFieldsList_Mapped.Add(appField)
			TargetFieldsList_Mapped.Add(appField.MappedTargetField)

		End Sub

		Public Sub MapTargetField(ByVal targetField As TargetField, ByVal appFieldToMapTo As AppField)
			'Checks
			CheckThatAnAppObjectHasBeenSelected()
			CheckThatAppFieldIsCurrentlySelected(appFieldToMapTo)
			CheckThatTargetFieldCanMapToAppField(targetField, appFieldToMapTo)
			CheckThatAppFieldIsNotAlreadyMappedToATaretField(appFieldToMapTo)
			CheckThatTargetFieldIsNotAlreadyMapped(targetField)
			CheckThatMapAppFieldMethodHasBeenCalledForAnAppField(appFieldToMapTo)

			'Update data
			appFieldToMapTo.MappedTargetField = targetField

			'Update lsits
			TargetFieldsList_NotMapped.Remove(targetField)
			Dim appMappedListIdx = AppFieldsList_Mapped.IndexOf(appFieldToMapTo)
			TargetFieldsList_Mapped(appMappedListIdx) = targetField
		End Sub

		Public Sub UnmapTargetField(ByVal targetField As TargetField)
			'Checks
			CheckThatAnAppObjectHasBeenSelected()
			CheckThatAnyAppFieldIsCurrentlySelected()
			CheckThatTargetFieldIsMappedToCurrentlySelectedAppField(targetField)

			'Update data
			_selectedAppFieldForMapping.MappedTargetField = targetField.Empty

			'Update lists
			Dim listIdx = TargetFieldsList_Mapped.IndexOf(targetField)
			TargetFieldsList_Mapped(listIdx) = _selectedAppFieldForMapping.MappedTargetField
			If Not targetField.IsEmpty Then
				TargetFieldsList_NotMapped.Add(targetField)
			End If

		End Sub

		Public Sub UnmapAppField(ByVal appField As AppField)
			'Checks
			CheckThatAnAppObjectHasBeenSelected()
			CheckThatAppFieldIsCurrentlySelected(appField)

			'Update data
			If Not appField.MappedTargetField IsNot Nothing AndAlso Not appField.MappedTargetField.IsEmpty Then
				UnmapTargetField(appField.MappedTargetField)
			End If
			_mappedByObjectTable(_selectedObject).Remove(appField)

			'Update lists
			Dim listIdx = AppFieldsList_Mapped.IndexOf(appField)
			TargetFieldsList_Mapped.RemoveAt(listIdx)
			AppFieldsList_Mapped.Remove(appField)
			AppFieldsList_NotMapped.Add(appField)

		End Sub

		Private Sub CheckThatTargetFieldIsMappedToCurrentlySelectedAppField(ByVal targetField As TargetField)
			If Not _selectedAppFieldForMapping.MappedTargetField Is targetField Then
				Throw New ArgumentException("Supplied TargetField is not mapped to the currently selected AppField")
			End If
		End Sub

		Private Sub CheckThatMapAppFieldMethodHasBeenCalledForAnAppField(ByVal appField As AppField)
			If Not _mappedByObjectTable(_selectedObject).Contains(appField) Then
				Throw New ArgumentException("To complete this operation, first call the MapAppField method on the supplied AppField")
			End If
		End Sub

		Private Sub CheckThatTargetFieldIsNotAlreadyMapped(ByVal targetField As TargetField)
			If targetField.IsEmpty Then
				Return 'We don't care about empty target fields
			End If
			If _selectedObject.AppFields.Where(Function(appField) appField.MappedTargetField Is targetField).Count() > 0 Then
				Throw New ArgumentException("Supplied TargetField is already mapped to an AppField")
			End If
		End Sub

		Private Sub CheckThatAppFieldIsNotAlreadyMappedToATaretField(ByVal appField As AppField)
			If appField.MappedTargetField IsNot Nothing AndAlso Not appField.MappedTargetField.IsEmpty Then
				Throw New ArgumentException("Supplied AppField is already mapped to a TargetField")
			End If
		End Sub

		Private Sub CheckThatTargetFieldCanMapToAppField(ByVal targetField As TargetField, ByVal appFieldToMapTo As AppField)
			If Not appFieldToMapTo.MappingCandidates.Contains(targetField) Then
				Throw New ArgumentException("Supplied TargetField is not a mapping candidate for the supplied AppField")
			End If
		End Sub

		Private Sub CheckThatAppFieldIsCurrentlySelected(ByVal appField As AppField)
			If Not _selectedAppFieldForMapping Is appField Then
				Throw New ArgumentException("Supplied AppField is not currently selected")
			End If
		End Sub

		Private Sub CheckThatAnyAppFieldIsCurrentlySelected()
			If _selectedAppFieldForMapping Is Nothing Then
				Throw New ArgumentException("To complete this operation, first select an AppField")
			End If
		End Sub

		Private Sub CheckThatAppFieldNotAlreadyInMappedList(ByVal appField As AppField)
			If _mappedByObjectTable(_selectedObject).Contains(appField) Then
				Throw New ArgumentException("Supplied AppField has already been added to the mapped list")
			End If
		End Sub

		Private Sub CheckThatAnAppObjectHasBeenSelected()
			'Make sure we've selected an object
			If _selectedObject Is Nothing Then
				Throw New Exception("No object has been selected yet")
			End If
		End Sub

		Private Sub CheckThatAppFieldBelongsToCurrentlySelectedObject(ByVal appField As AppField)
			'Check appField belongs to currently selected object
			If Not _selectedObject.AppFields.Contains(appField) Then
				Throw New ArgumentException("Supplied AppField is not contained by the currently selected AppObject")
			End If
		End Sub

		Private Function GetTargetFieldsAlreadyMapped(ByVal appObject As AppObject) As List(Of TargetField)
			Dim resultList As New List(Of TargetField)
			For Each appField In appObject.AppFields
				If Not appField.MappedTargetField.IsEmpty Then
					resultList.Add(appField.MappedTargetField)
				End If
			Next
			Return resultList
			'Return (From appField In _mappedByObjectTable(appObject) Where Not appField.MappedTargetField.IsEmpty Select appField.MappedTargetField).ToList()
		End Function

	End Class

End Namespace
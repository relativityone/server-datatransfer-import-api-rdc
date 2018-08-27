Imports System.Collections.Generic

Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class CustomFileNameDescriptorModel
		Private ReadOnly _firstField As FieldDescriptorPart
		Private ReadOnly _extendedDescriptors As ArrayList = New ArrayList(2)

		Public Sub New(ByVal firstField As FieldDescriptorPart, ByVal Optional firstExtendedDescriptor As ExtendedDescriptorPart = Nothing, ByVal Optional secondExtendedDescriptor As ExtendedDescriptorPart = Nothing)
			_firstField = firstField

			If firstExtendedDescriptor Is Nothing Then
				Return
			End If

			_extendedDescriptors.Add(firstExtendedDescriptor)

			If secondExtendedDescriptor Is Nothing Then
				Return
			End If

			_extendedDescriptors.Add(secondExtendedDescriptor)
		End Sub

		Public Iterator Function DescriptorParts() As IEnumerable(Of DescriptorPart)
			Yield _firstField

			For Each extendedDescriptorPart As ExtendedDescriptorPart In _extendedDescriptors
				Yield extendedDescriptorPart.Separator
				Yield extendedDescriptorPart.ValuePart
			Next
		End Function

		Public Function FirstFieldDescriptorPart() As FieldDescriptorPart
			Return _firstField
		End Function

		Public Function ExtendedDescriptorParts() As IList(Of ExtendedDescriptorPart)
			Return _extendedDescriptors.Cast(Of ExtendedDescriptorPart).ToList()
		End Function

	End Class
End Namespace

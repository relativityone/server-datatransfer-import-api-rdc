Imports System.Collections.Generic
Imports FileNaming.CustomFileNaming

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class FileNamePartProviderContainer
		Implements IFileNamePartProviderContainer

		Private ReadOnly _fileNamePartProviders As Dictionary(Of Type, IFileNamePartProvider) = New Dictionary(Of Type, IFileNamePartProvider)

		Public Sub New()
			Register(GetType(SeparatorDescriptorPart), New SeparatorFileNamePartProvider())
			Register(GetType(FieldDescriptorPart), New FieldFileNamePartProvider())
			Register(GetType(CustomTextDescriptorPart), New CustomTextFileNamePartProvider())
			Register(GetType(FirstFieldDescriptorPart), New FirstFieldFileNamePartProvider())
		End Sub


		Public Function GetProvider(descriptor As DescriptorPart) As IFileNamePartProvider Implements IFileNamePartProviderContainer.GetProvider
			Dim descriptorType As Type = descriptor.GetType()
			If Not _fileNamePartProviders.ContainsKey(descriptorType) Then
				Throw New ArgumentOutOfRangeException($"Can not find file name provider for descriptor: {descriptor.GetType()}")
			End If
			Return _fileNamePartProviders(descriptorType)
		End Function

		Public Sub Register(descriptorPartType As Type, provider As IFileNamePartProvider) Implements IFileNamePartProviderContainer.Register
			If Not descriptorPartType.IsSubclassOf(GetType(DescriptorPart)) Then
				Throw New ArgumentException($"File name registration part provider failed. Inavlid type arument: {descriptorPartType}")
			End If
			_fileNamePartProviders(descriptorPartType) = provider
		End Sub
	End Class
End Namespace

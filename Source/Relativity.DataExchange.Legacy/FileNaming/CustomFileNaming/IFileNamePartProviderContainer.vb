Imports FileNaming.CustomFileNaming

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Interface IFileNamePartProviderContainer

		Function GetProvider(descriptor As DescriptorPart) As IFileNamePartProvider

		Sub Register(descriptorPartType As Type, provider As IFileNamePartProvider)

	End Interface
End Namespace
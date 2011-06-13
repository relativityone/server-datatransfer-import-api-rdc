Imports System.Collections.Generic
Imports Relativity.Applications.Serialization.Elements

Namespace kCura.EDDS.WinForm.Data

	Public Interface IMappingCandidateFinder
		Sub PopulateMappingCandidates(ByVal appXml As System.Xml.XmlDocument, ByVal appMappingData As AppMappingData)
	End Interface

End Namespace
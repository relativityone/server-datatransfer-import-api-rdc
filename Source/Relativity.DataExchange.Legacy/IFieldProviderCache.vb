
Namespace kCura.WinEDDS

	Public Interface IFieldProviderCache

		Sub ResetCache()

		ReadOnly Property CurrentFields(artifactTypeId As Int32, workspaceId As Integer,
												Optional ByVal refresh As Boolean = False) As DocumentFieldCollection

		ReadOnly Property CurrentNonFileFields(artifactTypeId As Int32, workspaceId As Integer,
												Optional ByVal refresh As Boolean = False) As DocumentFieldCollection

	End Interface

End Namespace

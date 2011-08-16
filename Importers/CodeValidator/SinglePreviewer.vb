Namespace kCura.WinEDDS.CodeValidator
	Public Class SinglePreviewer
		Inherits Base

		Public Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal codeManager As kCura.WinEDDS.Service.CodeManager)
			Me.New(caseInfo, codeManager, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal codeManager As kCura.WinEDDS.Service.CodeManager, ByVal webURL As String)
			MyBase.New(caseInfo, codeManager, webURL)
		End Sub

		Public Overrides ReadOnly Property CreatedCodeCount() As Integer
			Get
				Return 0
			End Get
		End Property

		Protected Overrides ReadOnly Property DoRealtimeDatabaseLookup() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function GetNewSingleCodeId(ByVal field As Api.ArtifactField, ByVal codeName As String) As Nullable(Of Int32)
			Return New Nullable(Of Int32)(-1)
		End Function

	End Class
End Namespace


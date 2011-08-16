Namespace kCura.WinEDDS.CodeValidator
	Public Class SingleImporter
		Inherits Base
		Private _createdCodeCount As Int32 = 0

		Public Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal codeManager As kCura.WinEDDS.Service.CodeManager)
			MyBase.New(caseInfo, codeManager, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal codeManager As kCura.WinEDDS.Service.CodeManager, ByVal webURL As String)
			MyBase.New(caseInfo, codeManager, webURL)
		End Sub

		Protected Overrides ReadOnly Property DoRealtimeDatabaseLookup() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overloads Overrides Function GetNewSingleCodeId(ByVal field As Api.ArtifactField, ByVal codeName As String) As Nullable(Of Int32)
			Dim lookup As kCura.WinEDDS.Types.SingleChoiceCollection = DirectCast(Me.Lookup(field.CodeTypeID), kCura.WinEDDS.Types.SingleChoiceCollection)
			'TODO: 'Dim newCodeOrderValue As Int32 = System.Math.Min(lookup.MaxOrder, Int32.MaxValue - 1) + 1
			Dim newCodeOrderValue As Int32 = 0
			Dim code As kCura.EDDS.WebAPI.CodeManagerBase.Code = Me.CodeManager.CreateNewCodeDTOProxy(field.CodeTypeID, codeName, newCodeOrderValue, Me.CaseInfo.RootArtifactID)
			If code.Name.Length > 200 Then Throw New CodeCreationException(False, "Proposed choice name '" & code.Name & "' exceeds 200 characters, which is the maximum allowable.")
			Dim o As Object = Me.CodeManager.Create(Me.CaseInfo.ArtifactID, code)
			Dim codeArtifactID As Int32
			If TypeOf o Is Int32 Then
				codeArtifactID = CType(o, Int32)
			Else
				Throw New CodeCreationException(True, o.ToString)
			End If
			Select Case codeArtifactID
				Case -1
					Throw New CodeCreationException(True, codeName)
				Case -200
					Throw New CodeCreationException(True, "This choice field is not enabled as unicode.  Upload halted.")
			End Select
			Dim codeInfo As New Relativity.ChoiceInfo
			codeInfo.ArtifactID = codeArtifactID
			codeInfo.CodeTypeID = code.CodeType
			codeInfo.Name = code.Name
			codeInfo.Order = code.Order
			codeInfo.ParentArtifactID = code.ParentArtifactID.Value
			lookup.Add(codeInfo)
			_createdCodeCount += 1
			Return New Nullable(Of Int32)(codeArtifactID)
		End Function

		Public Overrides ReadOnly Property CreatedCodeCount() As Integer
			Get
				Return _createdCodeCount
			End Get
		End Property
	End Class
End Namespace

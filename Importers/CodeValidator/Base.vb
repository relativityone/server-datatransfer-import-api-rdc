Namespace kCura.WinEDDS.CodeValidator
	Public MustInherit Class Base
		Private _codeManager As kCura.WinEDDS.Service.CodeManager
		Private _lookup As New System.Collections.Hashtable
		Private _caseInfo As Relativity.CaseInfo

		Protected ReadOnly Property CodeManager() As kCura.WinEDDS.Service.CodeManager
			Get
				Return _codeManager
			End Get
		End Property

		Protected ReadOnly Property CaseInfo() As Relativity.CaseInfo
			Get
				Return _caseInfo
			End Get
		End Property

		Protected Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal codeManager As kCura.WinEDDS.Service.CodeManager)
			_codeManager = codeManager
			_caseInfo = caseInfo
		End Sub

		Public Function ValidateSingleCode(ByVal field As Api.ArtifactField, ByVal codeName As String) As Nullable(Of Int32)
			If codeName.Trim = String.Empty Then Return Nothing
			'TODO: Is this ever actually hit? ------ 'If field.CodeTypeID.IsNull Then Throw New kCura.WinEDDS.LoadFileBase.MissingCodeTypeException(Me.CurrentLineNumber, column)
			If Not _lookup.Contains(field.CodeTypeID) Then Me.InitializeLookupForCodeType(field.CodeTypeID)
			Dim typeLookup As kCura.WinEDDS.Types.SingleChoiceCollection = DirectCast(_lookup(field.CodeTypeID), kCura.WinEDDS.Types.SingleChoiceCollection)
			Dim choice As Relativity.ChoiceInfo = typeLookup(codeName)
			If Not choice Is Nothing Then Return New Nullable(Of Int32)(choice.ArtifactID)
			If Me.DoRealtimeDatabaseLookup Then choice = Me.CodeManager.RetrieveCodeByNameAndTypeID(Me.CaseInfo.ArtifactID, field.CodeTypeID, codeName.Trim)
			If choice Is Nothing Then
				Return Me.GetNewSingleCodeId(field, codeName)
			Else
				typeLookup.Add(choice)
				Return New Nullable(Of Int32)(choice.ArtifactID)
			End If
		End Function

		Protected ReadOnly Property Lookup(ByVal codeTypeID As Int32) As Object
			Get
				Return _lookup(codeTypeID)
			End Get
		End Property

		Private Sub InitializeLookupForCodeType(ByVal codeTypeID As Int32)
			Dim subLookup As New kCura.WinEDDS.Types.SingleChoiceCollection
			_lookup.Add(codeTypeID, subLookup)
			Try
				For Each choice As Relativity.ChoiceInfo In Me.CodeManager.RetrieveAllCodesOfType(Me.CaseInfo.ArtifactID, codeTypeID)
					subLookup.Add(choice)
				Next
			Catch
				'Do nothing; we can populate with individual reads
			End Try

		End Sub


		Public MustOverride Function GetNewSingleCodeId(ByVal field As Api.ArtifactField, ByVal codeName As String) As Nullable(Of Int32)
		Protected MustOverride ReadOnly Property DoRealtimeDatabaseLookup() As Boolean
		Public MustOverride ReadOnly Property CreatedCodeCount() As Int32

	End Class
End Namespace


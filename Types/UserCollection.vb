Namespace kCura.WinEDDS
	Public Class UserCollection
		Private _ht As System.Collections.Hashtable

		Default Public ReadOnly Property Item(ByVal emailAddress As String) As NullableTypes.NullableInt32
			Get
				If _ht.ContainsKey(emailAddress.ToLower) Then
					Return New NullableTypes.NullableInt32(CType(_ht(emailAddress.ToLower), Int32))
				Else
					Return NullableTypes.NullableInt32.Null
				End If
			End Get
		End Property

		Public Sub New(ByVal userManager As kCura.WinEDDS.Service.UserManager, ByVal caseContextID As Int32)
			_ht = New System.Collections.Hashtable
			Dim dr As System.Data.DataRow
			For Each dr In userManager.RetrieveAllAssignableInCase(caseContextID).Tables(0).Rows
				If Not _ht.ContainsKey(dr("EmailAddress").ToString.ToLower) Then
					_ht.Add(dr("EmailAddress").ToString.ToLower, dr("ArtifactID"))
				End If
			Next
		End Sub
	End Class
End Namespace
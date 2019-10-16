Namespace Relativity.Desktop.Client
	Public Class Constants
		Private Shared _defaultEncodingIDs As Int32()
		Public Shared ReadOnly Property DefaultEncodings() As EncodingItem()
			Get

				If _defaultEncodingIDs Is Nothing Then _defaultEncodingIDs = New Int32() {1252, 1200, 1201, 65001}
				Dim retval As New List(Of EncodingItem)
				For Each item As EncodingItem In AllEncodings
					If System.Array.IndexOf(_defaultEncodingIDs, item.CodePageId) <> -1 Then
						retval.Add(item)
					End If
				Next
				Return retval.ToArray()
			End Get
		End Property

		Public Shared Sub AddDefaultEncoding(ByVal encodingItem As EncodingItem)
			If _defaultEncodingIDs Is Nothing Then _defaultEncodingIDs = New Int32() {1252, 1200, 1201, 65001}
			Dim al As New List(Of Int32)(_defaultEncodingIDs)
			If Not al.Contains(encodingItem.CodePageId) Then al.Add(encodingItem.CodePageId)
			_defaultEncodingIDs = al.ToArray()
		End Sub
		Private Shared _allEncodings As EncodingItem() = Nothing
		Public Shared ReadOnly Property AllEncodings() As EncodingItem()
			Get
				If _allEncodings Is Nothing Then
					Dim al As New List(Of EncodingItem)
					For Each e As System.Text.EncodingInfo In System.Text.Encoding.GetEncodings
						If e.CodePage = 12001 OrElse e.CodePage = 12000 Then
							'NOT SUPPORTING UTF-32/UTF-32 BIG ENDIAN
							Continue For
						End If
						al.Add(EncodingItem.GetEncodingItemFromCodePageId(e.CodePage))
					Next

					While al.Contains(Nothing)
						al.Remove(Nothing)
					End While
					_allEncodings = al.ToArray()
					System.Array.Sort(_allEncodings, New EncodingItem.EncodingItemComparer)
				End If
				Return _allEncodings
			End Get
		End Property

	End Class
End Namespace
Namespace kCura.EDDS.WinForm
	Public Class Constants
		Private Shared _defaultEncodingIDs As Int32()
		Public Shared ReadOnly Property DefaultEncodings() As EncodingItem()
			Get

				If _defaultEncodingIDs Is Nothing Then _defaultEncodingIDs = New Int32() {1252, 1200, 1201, 65001}
				Dim retval As New System.Collections.ArrayList
				For Each item As EncodingItem In AllEncodings
					If System.Array.IndexOf(_defaultEncodingIDs, item.CodePageId) <> -1 Then
						retval.Add(item)
					End If
				Next
				Return DirectCast(retval.ToArray(GetType(EncodingItem)), EncodingItem())
			End Get
		End Property

		Public Shared Sub AddDefaultEncoding(ByVal encodingItem As EncodingItem)
			If _defaultEncodingIDs Is Nothing Then _defaultEncodingIDs = New Int32() {1252, 1200, 1201, 65001}
			Dim al As New System.Collections.ArrayList(_defaultEncodingIDs)
			If Not al.Contains(encodingItem.CodePageId) Then al.Add(encodingItem.CodePageId)
			_defaultEncodingIDs = DirectCast(al.ToArray(GetType(Int32)), Int32())
		End Sub
		Private Shared _allEncodings As EncodingItem() = Nothing
		Public Shared ReadOnly Property AllEncodings() As EncodingItem()
			Get
				If _allEncodings Is Nothing Then
					Dim al As New System.Collections.ArrayList
					For Each e As System.Text.EncodingInfo In System.Text.Encoding.GetEncodings
						If e.CodePage = 12001 OrElse e.CodePage = 12000 Then	'NOT SUPPORTING UTF-32/UTF-32 BIG ENDIAN
							Continue For
						End If
						al.Add(EncodingItem.GetEncodingItemFromCodePageId(e.CodePage))
					Next

					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(37))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(290))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(300))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(437))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(500))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(708))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(709))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(710))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(711))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(720))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(737))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(775))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(833))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(834))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(835))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(836))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(837))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(850))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(852))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(855))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(857))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(860))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(861))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(862))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(863))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(864))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(865))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(866))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(869))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(870))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(874))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(875))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(932))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(936))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(949))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(950))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1026))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1027))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1200))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1201))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1250))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1251))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1252))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1253))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1254))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1255))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1256))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1257))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1258))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(1361))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10000))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10001))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10002))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10003))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10004))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10005))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10006))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10007))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10008))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10010))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10017))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10029))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10079))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10081))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(10082))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20105))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20106))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20107))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20108))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20261))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20269))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20273))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20277))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20278))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20280))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20284))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20285))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20290))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20297))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20420))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20423))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20833))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20838))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20866))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20871))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20880))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(20905))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(21025))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(21027))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(21866))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28591))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28592))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28593))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28594))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28595))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28596))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28597))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28598))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(28599))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(29001))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50000))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50220))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50221))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50222))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50225))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50932))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(50949))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(51932))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(51949))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(52936))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57002))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57003))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57004))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57005))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57006))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57007))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57008))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57009))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57010))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(57011))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(65000))
					'al.Add(EncodingItem.GetEncodingItemFromCodePageId(65001))
					While al.Contains(Nothing)
						al.Remove(Nothing)
					End While
					_allEncodings = DirectCast(al.ToArray(GetType(EncodingItem)), EncodingItem())
					System.Array.Sort(_allEncodings, New EncodingItem.EncodingItemComparer)
				End If
				Return _allEncodings
			End Get
		End Property

	End Class
End Namespace


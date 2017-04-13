Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization

Namespace kCura.WinEDDS
	<Serializable()> Public Class ViewFieldInfo
		Inherits Relativity.ViewFieldInfo
		Implements IComparable

		Public Sub New(ByVal row As System.Data.DataRow)
			MyBase.New(row)
		End Sub
		Public Sub New(ByVal vfi As Relativity.ViewFieldInfo)
			MyBase.New(vfi)
		End Sub

		Public Overrides Function ToString() As String
			Return Me.DisplayName
		End Function

		Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
			Return String.Compare(Me.DisplayName, obj.ToString)
		End Function

		Public Shadows Function Equals(ByVal other As ViewFieldInfo) As Boolean
			If Me.AvfId = other.AvfId AndAlso Me.AvfColumnName = other.AvfColumnName Then Return True
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 45 * Me.AvfId
		End Function
		
		<NonSerialized>
		Public IsExportable As Boolean = True

	End Class

	Module StringExtensions
	<Extension()>
	Public Function Exportable(viewFieldsInfo As ArrayList) As IEnumerable(Of ViewFieldInfo)
		Return viewFieldsInfo.Cast(Of ViewFieldInfo).Where(Function(x) x.IsExportable)
	End Function

	End Module
End Namespace

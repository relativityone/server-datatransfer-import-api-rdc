Imports System.Collections.Generic

Namespace kCura.WinEDDS
	<Serializable()>
	Public Class ExtendedExportFile
		Inherits ExportFile
		Implements System.Runtime.Serialization.ISerializable

		Public SelectedNativesNameViewFields As IList(Of ViewFieldInfo)

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			info.AddValue("SelectedNativesNameViewFields", Me.SelectedNativesNameViewFields.ToArray(), GetType(ViewFieldInfo()))
			MyBase.GetObjectData(info, context)
		End Sub


		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
			Dim SelectedNativesNameViewFieldsAsArray As ViewFieldInfo() = Nothing
			Try
				SelectedNativesNameViewFieldsAsArray = DirectCast(info.GetValue("SelectedNativesNameViewFields", GetType(ViewFieldInfo())), ViewFieldInfo())
			Catch
				Dim field As kCura.WinEDDS.ViewFieldInfo = DirectCast(info.GetValue("SelectedNativesNameViewFields", GetType(kCura.WinEDDS.ViewFieldInfo)), kCura.WinEDDS.ViewFieldInfo)
				SelectedNativesNameViewFieldsAsArray = If(field Is Nothing, Nothing, {field})
			End Try
			Me.SelectedNativesNameViewFields = SelectedNativesNameViewFieldsAsArray.ToList()
		End Sub


		Public Sub New(artifactTypeID As Integer)
			MyBase.New(artifactTypeID)
			SelectedNativesNameViewFields = New List(Of ViewFieldInfo)
		End Sub
	End Class
End Namespace

Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions

Namespace kCura.WinEDDS
	Class RdcFileSerializationSurrogate
		Implements ISerializationSurrogate
		Public Function ISerializationSurrogate_SetObjectData(obj As Object, info As SerializationInfo, context As StreamingContext, selector As ISurrogateSelector) As Object Implements ISerializationSurrogate.SetObjectData
			If obj Is Nothing Then
				Throw New ArgumentNullException(NameOf(obj))
			End If
			If info Is Nothing Then
				Throw New ArgumentNullException(NameOf(info))
			End If

			Dim type As Type = obj.GetType()
			Dim regex As Regex = New Regex("(\w+\+)?_?(\w+)")
			For Each entry As SerializationEntry In info
				Dim match As Match = regex.Match(entry.Name)
				Dim propertyInfo As PropertyInfo = type.GetProperty(match.Groups(2).Value, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.IgnoreCase)
				If propertyInfo Is Nothing Then
					Throw New Exception()
				End If

				Dim value As Object = If(propertyInfo.PropertyType.IsEnum, [Enum].Parse(propertyInfo.PropertyType, entry.Value.ToString()), CTypeDynamic(entry.Value, propertyInfo.PropertyType))
				propertyInfo.SetMethod.Invoke(obj, New Object() {value})
			Next
			Return obj
		End Function

		Public Sub ISerializationSurrogate_GetObjectData(obj As Object, info As SerializationInfo, context As StreamingContext) Implements ISerializationSurrogate.GetObjectData
			Throw New NotImplementedException()
		End Sub
	End Class
End Namespace

Imports System.Reflection

Namespace kCura.WinEDDS
	Public Class RdcFileSerializationBinder
		Inherits System.Runtime.Serialization.SerializationBinder

		Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
			Return GetType(kCura.WinEDDS.ExportFile).Assembly.GetType(typeName)
		End Function
	End Class
End Namespace


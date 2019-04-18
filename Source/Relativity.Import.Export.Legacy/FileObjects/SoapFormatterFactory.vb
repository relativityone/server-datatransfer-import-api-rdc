Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Soap

Namespace kCura.WinEDDS
	Public Class SoapFormatterFactory
		Public Shared Function Create() As SoapFormatter
			Dim surrogateSelector As SurrogateSelector = New SurrogateSelector()
			surrogateSelector.AddSurrogate(GetType(kCura.WinEDDS.ViewFieldInfo), New StreamingContext(StreamingContextStates.All), New RdcFileSerializationSurrogate())


			Dim soapFormatter As SoapFormatter = New SoapFormatter(surrogateSelector, New StreamingContext(StreamingContextStates.All))
			soapFormatter.Binder = New RdcFileSerializationBinder()
			Return soapFormatter
		End Function
	End Class
End Namespace
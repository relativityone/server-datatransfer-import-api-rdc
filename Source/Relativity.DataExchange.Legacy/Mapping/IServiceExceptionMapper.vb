Imports System.Web.Services.Protocols
Imports Relativity.Services.Exceptions

Namespace kCura.WinEDDS.Mapping
	Public Interface IServiceExceptionMapper
		Function Map(serviceException As ServiceException) As SoapException
	End Interface
End Namespace

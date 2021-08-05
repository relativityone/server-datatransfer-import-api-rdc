Imports System.IO
Imports System.Web.Services.Protocols
Imports System.Xml
Imports System.Xml.Serialization
Imports Relativity.DataExchange.Logger
Imports Relativity.Logging
Imports Relativity.Services.Exceptions

Namespace kCura.WinEDDS.Mapping

	''' <summary>
	''' Maps Kepler ServiceException to SoapException which is handled by RDC and IAPI code.
	''' </summary>
    Public Class KeplerExceptionMapper
	    Implements IServiceExceptionMapper

        Private ReadOnly _logger As ILog = RelativityLogger.Instance

        Public Function Map(serviceException As ServiceException) As SoapException Implements IServiceExceptionMapper.Map
	        Return Soapify(serviceException)
        End Function

        Private Function Soapify(exception As Exception) As SoapException
	        Try 
		        Dim details As New KeplerExceptionDetail(exception)
	        
		        Dim xmlDocument As New XmlDocument
		        Dim xmlSerializer As New XmlSerializer(GetType(KeplerExceptionDetail))
            
		        Dim ns As New XmlSerializerNamespaces()
		        ns.Add(String.Empty, String.Empty)
            
		        Using stringWriter As New StringWriter()
			        xmlSerializer.Serialize(stringWriter, details, ns)
			        xmlDocument.LoadXml(stringWriter.ToString)
		        End Using

		        Return New SoapException(exception.Message, SoapException.ServerFaultCode, String.Empty, xmlDocument.ChildNodes(1), exception)
	        Catch ex As Exception
		        _logger.LogError(ex, "Error when mapping Kepler exception - {ExceptionMessage}. Kepler exception: {KeplerExceptionMessage}", ex.Message, exception.Message)
		        Return New SoapException(exception.Message, SoapException.ServerFaultCode, String.Empty, Nothing, ex)
	        End Try
        End Function
    End Class
End Namespace
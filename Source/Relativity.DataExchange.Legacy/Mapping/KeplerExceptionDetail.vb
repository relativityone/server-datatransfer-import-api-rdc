Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Service
Imports Relativity.Logging

Namespace kCura.WinEDDS.Mapping

    ''' <summary>
    ''' Represents the SoapException detail property extracted from Kepler ServiceException message.
    ''' Please also refer <seealso cref="SoapExceptionDetail"/>
    ''' </summary>
    <Serializable>
    <Xml.Serialization.XmlType("SoapExceptionDetail")>
    <Xml.Serialization.XmlRoot(ElementName:="detail")>
    Public Class KeplerExceptionDetail

        Private Const ExceptionTypeKey As String = "InnerExceptionType:"
        Private Const ExceptionMessageKey As String = "InnerExceptionMessage:"
        Private Const ExceptionMessageValuePrefix As String = "Error:"
        Private Const KeyValuePairSeparator As Char = CChar(",")

        Private ReadOnly _logger As ILog = RelativityLogger.Instance

        <Xml.Serialization.XmlElement("ExceptionType")>
        Public Property ExceptionType As String

        <Xml.Serialization.XmlElement("ExceptionMessage")>
        Public Property ExceptionMessage As String

        <Xml.Serialization.XmlElement("ExceptionFullText")>
        Public Property ExceptionFullText As String

        Public Sub New()
        End Sub

        Public Sub New(exception As Exception)
            ExtractDetailsFromException(exception)
        End Sub

        ''' <summary>
        ''' Extracts InnerExceptionType and InnerExceptionMessage from exception message text.
        ''' Example exception message to be parsed: "Error during call BulkImportImageAsync. InnerExceptionType: Relativity.Core.Exception.InsufficientAccessControlListPermissions, InnerExceptionMessage: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission."
        ''' </summary>
        ''' <param name="exception">Exception to parse.</param>
        Private Sub ExtractDetailsFromException(exception As Exception)
	        Try
		        SetDetailsFromException(exception)

                If ShouldParseExceptionMessage(exception)
	                SetDetailsFromExceptionMessage(exception)
                End If
            Catch ex As Exception
	            _logger.LogError(ex, "Error when parsing Kepler error message - {ExceptionMessage}. Kepler error message: {KeplerExceptionMessage}", ex.Message, exception.Message)
            End Try
        End Sub

        Private Function ShouldParseExceptionMessage(exception As Exception) As Boolean
	        If String.IsNullOrWhiteSpace(exception?.Message)
		        Return False
	        End If

	        Dim exceptionTypeIndex As Integer = exception.Message.IndexOf(ExceptionTypeKey, StringComparison.OrdinalIgnoreCase)
	        Dim exceptionMessageIndex As Integer = exception.Message.IndexOf(ExceptionMessageKey, StringComparison.OrdinalIgnoreCase)

	        If (exceptionTypeIndex = -1 Or exceptionMessageIndex = -1)
		        Return False
	        End If

	        Return True
        End Function

        Private Sub SetDetailsFromException(exception As Exception)
	        If exception Is Nothing
		        ExceptionType = String.Empty
		        ExceptionMessage = String.Empty
		        ExceptionFullText = String.Empty
	        Else
		        ExceptionType = exception.GetType().FullName
		        ExceptionMessage = exception.Message?.Trim()
		        ExceptionFullText = ExceptionMessage
	        End If
        End Sub

        Private Sub SetDetailsFromExceptionMessage(exception As Exception)
	        Dim exceptionTypeIndex As Integer = exception.Message.IndexOf(ExceptionTypeKey, StringComparison.OrdinalIgnoreCase)
	        Dim exceptionMessageIndex As Integer = exception.Message.IndexOf(ExceptionMessageKey, StringComparison.OrdinalIgnoreCase)

	        ExceptionType = ExtractKeyValueFromText(exception.Message, exceptionTypeIndex, ExceptionTypeKey.Length, exceptionMessageIndex)
	        ExceptionMessage = ExtractKeyValueFromText(exception.Message, exceptionMessageIndex, ExceptionMessageKey.Length, exceptionTypeIndex, ExceptionMessageValuePrefix)
	        ExceptionFullText = ExceptionMessage
        End Sub

        Private Function ExtractKeyValueFromText(inputText As String, keyIndex As Integer, keyLength As Integer, endIndex As Integer, Optional valuePrefix As String = Nothing) As String

            Dim extractedValue As String

            If keyIndex = -1
                Return String.Empty
            End If

            If endIndex < keyIndex
                extractedValue = inputText.Substring(keyIndex + keyLength).Trim()
            Else
                extractedValue = inputText.Substring(keyIndex + keyLength, endIndex - keyIndex - keyLength).Trim()
            End If

            RemoveLastCharacter(extractedValue, KeyValuePairSeparator)
            AppendPrefix(extractedValue, valuePrefix)

            Return extractedValue.Trim()
        End Function

        Private Sub RemoveLastCharacter(ByRef inputText As String, lastCharToRemove As Char)
            If String.IsNullOrEmpty(inputText)
                Return
            End If

            Dim inputTextLastChar As Char = inputText(inputText.Length - 1)
            If inputTextLastChar = lastCharToRemove
                inputText = inputText.Remove(inputText.Length - 1, 1)
            End If
        End Sub

        Private Sub AppendPrefix(ByRef inputText As String, prefix As String)
            If String.IsNullOrWhiteSpace(inputText)
                Return
            End If

            If String.IsNullOrWhiteSpace(prefix)
                Return
            End If

            inputText = prefix + " " + inputText
        End Sub
    End Class
End Namespace

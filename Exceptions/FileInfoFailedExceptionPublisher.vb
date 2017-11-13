﻿Imports kCura.WinEDDS.TApi

Namespace Exceptions

    Public Class FileInfoFailedExceptionPublisher
        Implements IFileInfoFailedExceptionPublisher

        Public Sub ThrowNewException(message As String) Implements IFileInfoFailedExceptionPublisher.ThrowNewException
            Throw New FileInfoFailedException(message)
        End Sub
    End Class
End NameSpace
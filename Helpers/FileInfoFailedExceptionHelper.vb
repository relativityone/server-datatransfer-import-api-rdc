﻿Imports kCura.WinEDDS.TApi

Namespace Helpers

    Public Class FileInfoFailedExceptionHelper
        Implements IFileInfoFailedExceptionHelper

        Public Sub ThrowNewException(message As String) Implements IFileInfoFailedExceptionHelper.ThrowNewException
            Throw New FileInfoFailedException(message)
        End Sub
    End Class
End NameSpace
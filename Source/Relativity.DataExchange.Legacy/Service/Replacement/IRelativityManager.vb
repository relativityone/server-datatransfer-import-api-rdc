Namespace kCura.WinEDDS.Service.Replacement
    Public interface IRelativityManager
        Inherits IDisposable
        Shadows Function RetrieveCurrencySymbol() As String
        Shadows Function IsImportEmailNotificationEnabled() As Boolean
        Shadows Function RetrieveRdcConfiguration() As System.Data.DataSet
        Shadows Function ValidateSuccessfulLogin() As Boolean
        Shadows Function ValidateCertificate() As Boolean
        Shadows Function RetrieveCurrencySymbolV2() As String
    End interface
End NameSpace
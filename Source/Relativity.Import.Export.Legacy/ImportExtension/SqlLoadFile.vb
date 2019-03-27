Namespace kCura.WinEDDS.ImportExtension
    'ByVal serverName As String, ByVal databaseName As String, ByVal loginName As String, ByVal loginPassword As String, ByVal query As String)
    Public Class SqlLoadFile
        Inherits kCura.WinEDDS.LoadFile

        Private _serverName As String
        Private _databaseName As String
        Private _loginName As String
        Private _loginPassword As String
        Private _query As String

        Public Property ServerName() As String
            Get
                Return _serverName
            End Get
            Set(ByVal Value As String)
                _serverName = Value
            End Set
        End Property

        Public Property DatabaseName() As String
            Get
                Return _databaseName
            End Get
            Set(ByVal Value As String)
                _databaseName = Value
            End Set
        End Property

        Public Property LoginName() As String
            Get
                Return _loginName
            End Get
            Set(ByVal Value As String)
                _loginName = Value
            End Set
        End Property

        Public Property LoginPassword() As String
            Get
                Return _loginPassword
            End Get
            Set(ByVal Value As String)
                _loginPassword = Value
            End Set
        End Property

        Public Property Query() As String
            Get
                Return _query
            End Get
            Set(ByVal Value As String)
                _query = Value
            End Set
        End Property

    End Class
End Namespace


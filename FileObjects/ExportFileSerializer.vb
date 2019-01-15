Imports System.Xml.Linq
Namespace kCura.WinEDDS
	Public Class ExportFileSerializer

		Private Const ENTITY_OBJECT_TYPE_NAME As String = "Entity" 
		Private Const CUSTODIAN_OBJECT_TYPE_NAME As String = "Custodian" 

		Private _settingsValidator As New ExportSettingsValidator
		Public Property SettingsValidator As ExportSettingsValidator
			Get
				If _settingsValidator Is Nothing Then _settingsValidator = New ExportSettingsValidator
				Return _settingsValidator
			End Get
			Set(ByVal value As ExportSettingsValidator)
				_settingsValidator = value
			End Set
		End Property

		Public Overridable Function TransformExportFileXml(ByVal input As XDocument) As String
			Return input.ToString
		End Function

		Public Overridable Function DeserializeExportFile(ByVal currentExportFile As kCura.WinEDDS.ExportFile, ByVal xml As String) As kCura.WinEDDS.ExtendedExportFile
			Dim deserialized As kCura.WinEDDS.ExportFile = Me.DeserializeExportFile(XDocument.Parse(xml))
			Return PopulateDeserializedExportFile(currentExportFile, deserialized)
		End Function

		Public Overridable Function PopulateDeserializedExportFile(ByVal currentExportFile As kCura.WinEDDS.ExportFile, deserialized As ExportFile) As kCura.WinEDDS.ExtendedExportFile
			Dim retval As New kCura.WinEDDS.ExtendedExportFile(currentExportFile.ArtifactTypeID)
			For Each p As System.Reflection.PropertyInfo In (From prop As System.Reflection.PropertyInfo In retval.GetType.GetProperties Where prop.CanWrite)
				p.SetValue(retval, p.GetValue(If(PropertyIsReadFromExisting(p), currentExportFile, deserialized), Nothing), Nothing)
			Next
			Select Case retval.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch
					retval.ArtifactID = currentExportFile.ArtifactID
				Case ExportFile.ExportType.Production
					retval.ImagePrecedence = New Pair() {}
			End Select
			If Not ObjectTypesAreCompatible(currentExportFile, retval) Then
				retval = New ErrorExportFile("Cannot load '" & currentExportFile.ObjectTypeName & "' settings from a saved '" & retval.ObjectTypeName & "' export")
			End If
			If Not Me.SettingsValidator.IsValidExportDirectory(retval.FolderPath) Then retval.FolderPath = String.Empty
			Return retval
		End Function

		Public Function ObjectTypesAreCompatible(ByVal currentExportFile As ExportFile, ByVal deserializedExportFile As ExportFile) As Boolean
			If currentExportFile.ObjectTypeName = ENTITY_OBJECT_TYPE_NAME AndAlso deserializedExportFile.ObjectTypeName = CUSTODIAN_OBJECT_TYPE_NAME Then
				Return True
			End If
			If currentExportFile.ObjectTypeName = CUSTODIAN_OBJECT_TYPE_NAME AndAlso deserializedExportFile.ObjectTypeName = ENTITY_OBJECT_TYPE_NAME Then
				Return True
			End If
			Return Relativity.SqlNameHelper.GetSqlFriendlyName(currentExportFile.ObjectTypeName) = Relativity.SqlNameHelper.GetSqlFriendlyName(deserializedExportFile.ObjectTypeName)
		End Function

		Private Function PropertyIsReadFromExisting(ByVal p As System.Reflection.PropertyInfo) As Boolean
			For Each att As Attribute In p.GetCustomAttributes(GetType(ReadFromExisting), False)
				Return True
			Next
			Return False
		End Function

		Public Overridable Function DeserializeExportFile(ByVal xml As XDocument) As ExportFile
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Dim cleansedInput As String = Me.TransformExportFileXml(xml)
			Dim sr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(cleansedInput))
			Dim deserialized As kCura.WinEDDS.ExportFile = Nothing
			Try
				deserialized = DirectCast(deserializer.Deserialize(sr), WinEDDS.ExportFile)
			Catch
				Throw
			Finally
				sr.Close()
			End Try
			Return deserialized
		End Function

		Public Class ExportSettingsValidator
			Public Overridable Function IsValidExportDirectory(ByVal path As String) As Boolean
				Return System.IO.Directory.Exists(path)
			End Function
		End Class
	End Class
End Namespace


Imports NullableTypes
Namespace kCura.WinEDDS.NUnit
	Public Class LoadFileHelper

#Region "Constants"
		Public Shared CASEARTIFACTID As Int32 = 2
		Public Shared CASENAME As String = "Case Name"
		Public Shared CASEMATTERARTIFACTID As Int32 = 3
		Public Shared CASESTATUSCODEARTIFACTID As Int32 = 4
		Public Shared CASEROOTFOLDERID As Int32 = 5
		Public Shared CASEROOTARTIFACTID As Int32 = 6
		Public Shared DESTINATIONFOLDERID As Int32 = 7
		Public Shared LOADFILEPATH As String = "C:\SourceCode\EDDS\trunk\kCura.EDDS.WinForm.UnitTest\Fixtures\SampleLoadFile.txt"
#End Region

		Public Function GenerateEmptyDocFieldCollection() As kCura.WinEDDS.DocumentFieldCollection
			Return New kCura.WinEDDS.DocumentFieldCollection
		End Function

		Public Function CodeTypeColumns() As DataColumn()
			Return New DataColumn() { _
			 New DataColumn("CodeTypeID", GetType(Int32)), _
			 New DataColumn("Name", GetType(String)), _
			 New DataColumn("DisplayName", GetType(String)), _
			 New DataColumn("IsEditable", GetType(Boolean))}
		End Function

		Public Function CodeColumns() As DataColumn()
			Return New DataColumn() { _
			 New DataColumn("ArtifactID", GetType(Int32)), _
			 New DataColumn("Name", GetType(String)), _
			 New DataColumn("Order", GetType(Int32)), _
			 New DataColumn("CodeTypeID", GetType(Int32)), _
			 New DataColumn("IsActive", GetType(Boolean)), _
			 New DataColumn("CodeType", GetType(String)), _
			 New DataColumn("CodeTypeDisplayName", GetType(String)), _
			 New DataColumn("CreatedByName", GetType(String)), _
			 New DataColumn("LastModifiedByName", GetType(String)), _
			 New DataColumn("LastModifiedBy", GetType(Int32)), _
			 New DataColumn("CreatedBy", GetType(Int32)), _
			 New DataColumn("CreatedOn", GetType(DateTime)), _
			 New DataColumn("LastModifiedOn", GetType(DateTime)), _
			 New DataColumn("Keywords", GetType(String)), _
			 New DataColumn("Notes", GetType(String)) _
			}
		End Function

		Public Function GenerateSampleCodeDataTable() As DataTable
			Dim dt As New DataTable
			dt.Columns.AddRange(CodeColumns)

			Dim row As New ArrayList
			row.Add(401)
			row.Add("Alpha")
			row.Add(0)
			row.Add(1)
			row.Add(True)
			row.Add("SingleCodeOne")
			row.Add("Single Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(402)
			row.Add("Beta")
			row.Add(1)
			row.Add(1)
			row.Add(True)
			row.Add("SingleCodeOne")
			row.Add("Single Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(403)
			row.Add("Gamma")
			row.Add(1)
			row.Add(1)
			row.Add(True)
			row.Add("SingleCodeOne")
			row.Add("Single Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(404)
			row.Add("Alef")
			row.Add(1)
			row.Add(2)
			row.Add(True)
			row.Add("SingleCodeTwo")
			row.Add("Single Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(405)
			row.Add("Bet")
			row.Add(1)
			row.Add(2)
			row.Add(True)
			row.Add("SingleCodeTwo")
			row.Add("Single Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(406)
			row.Add("Gimmel")
			row.Add(1)
			row.Add(2)
			row.Add(True)
			row.Add("SingleCodeTwo")
			row.Add("Single Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(407)
			row.Add("Fehu")
			row.Add(0)
			row.Add(3)
			row.Add(True)
			row.Add("MultiCodeOne")
			row.Add("Multi Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(408)
			row.Add("Uruz")
			row.Add(1)
			row.Add(3)
			row.Add(True)
			row.Add("MultiCodeOne")
			row.Add("Multi Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(409)
			row.Add("Thurisaz")
			row.Add(1)
			row.Add(3)
			row.Add(True)
			row.Add("MultiCodeOne")
			row.Add("Multi Code One")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(410)
			row.Add("Alpha")
			row.Add(1)
			row.Add(4)
			row.Add(True)
			row.Add("MultiCodeTwo")
			row.Add("Multi Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(411)
			row.Add("Veeta")
			row.Add(1)
			row.Add(4)
			row.Add(True)
			row.Add("MultiCodeTwo")
			row.Add("Multi Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)

			row = New ArrayList
			row.Add(412)
			row.Add("Ghamma")
			row.Add(1)
			row.Add(4)
			row.Add(True)
			row.Add("MultiCodeTwo")
			row.Add("Multi Code Two")
			row.Add("Test User")
			row.Add("Test User")
			row.Add(1000)
			row.Add(1000)
			row.Add(DateTime.Now)
			row.Add(DateTime.Now)
			row.Add("")
			row.Add("")
			dt.Rows.Add(row.ToArray)
			Return dt
		End Function

		Public Function GenerateSampleCodeTypeDataTable() As DataTable
			Dim dt As New DataTable
			dt.Columns.AddRange(CodeTypeColumns)
			Dim al As New ArrayList
			al.Add(1)
			al.Add("SingleCodeOne")
			al.Add("Single Code One")
			al.Add(False)
			dt.Rows.Add(al.ToArray)
			al = New ArrayList
			al.Add(2)
			al.Add("SingleCodeTwo")
			al.Add("Single Code Two")
			al.Add(False)
			dt.Rows.Add(al.ToArray)
			al = New ArrayList
			al.Add(3)
			al.Add("MultiCodeOne")
			al.Add("Multi Code One")
			al.Add(False)
			dt.Rows.Add(al.ToArray)
			al = New ArrayList
			al.Add(4)
			al.Add("MultiCodeTwo")
			al.Add("Multi Code Two")
			al.Add(False)
			dt.Rows.Add(al.ToArray)
			Return dt
		End Function

		Public Function GetSampleSelectedFields() As kCura.WinEDDS.LoadFileFieldMap
			Dim docfields As kCura.WinEDDS.DocumentField() = { _
			 New kCura.WinEDDS.DocumentField("VarCharGeneric", 101, 0, 0, NullableInt32.Null, New NullableInt32(255)), _
			 New kCura.WinEDDS.DocumentField("VarCharIdentifier", 102, 0, 2, NullableInt32.Null, New NullableInt32(255)), _
			 New kCura.WinEDDS.DocumentField("TextGeneric", 103, 4, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("TextFullText", 104, 4, 1, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Integer", 105, 1, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Date", 106, 2, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Boolean", 107, 3, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Code", 108, 5, 0, New NullableInt32(1), NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Decimal", 109, 6, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("Currency", 110, 7, 0, NullableInt32.Null, NullableInt32.Null), _
			 New kCura.WinEDDS.DocumentField("MultiCode", 111, 8, 0, New NullableInt32(3), NullableInt32.Null) _
			}
			Dim retval As New kCura.WinEDDS.LoadFileFieldMap
			Dim i As Int32
			For i = 0 To docfields.Length - 1
				retval.Add(New LoadFileFieldMap.LoadFileFieldMapItem(docfields(i), i))
			Next
			Return retval
		End Function

		Public Enum FieldDescription
			VarCharGeneric = 0
			VarCharIdentifier = 1
			TextGeneric = 2
			TextFullText = 3
			[Integer] = 4
			[Date] = 5
			[Boolean] = 6
			Code = 7
			[Decimal] = 8
			Currency = 9
			MultiCode = 10
		End Enum

		Public Function GetSampleCaseInfo() As kCura.EDDS.Types.CaseInfo
			Dim ci As New kCura.EDDS.Types.CaseInfo
			ci.ArtifactID = CASEARTIFACTID
			ci.EmailAddress = ""
			ci.MatterArtifactID = CASEMATTERARTIFACTID
			ci.Name = CASENAME
			ci.RootArtifactID = CASEROOTARTIFACTID
			ci.RootFolderID = CASEROOTFOLDERID
			ci.StatusCodeArtifactID = CASESTATUSCODEARTIFACTID
			Return ci
		End Function

		Public Function GetSampleLoadFileObjectIgnoreUploading() As kCura.WinEDDS.LoadFile
			Dim retval As New kCura.WinEDDS.LoadFile
			retval.CaseInfo = GetSampleCaseInfo()
			retval.Credentials = Nothing
			retval.DestinationFolderID = DESTINATIONFOLDERID
			retval.ExtractFullTextFromNativeFile = False
			retval.FilePath = LOADFILEPATH
			retval.FirstLineContainsHeaders = True
			retval.LoadNativeFiles = False
			retval.MultiRecordDelimiter = ";"c
			retval.NativeFilePathColumn = "FilePath"
			retval.NewlineDelimiter = ChrW(174)
			retval.OverwriteDestination = "None"
			retval.QuoteDelimiter = ChrW(254)
			retval.RecordDelimiter = ChrW(20)
			retval.FieldMap = GetSampleSelectedFields()
			retval.SelectedIdentifierField = retval.FieldMap.DocumentFields(1)
			Return retval
		End Function
	End Class
End Namespace


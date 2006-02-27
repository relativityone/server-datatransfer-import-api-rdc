Imports kCura.Utility
Imports NUnit.Framework
Namespace kCura.EDDS.NUnit

	<TestFixture()> _
	Public Class Main

#Region "Declarations"
		Private _fixture As New Fixture
		Private _errorMessage As String
		Private _deleteSuccess As Boolean
#End Region

#Region "Test Methods"
		''' -----------------------------------------------------------------------------
		''' <summary>
		''' This method called only once, immediatly when the test starts.
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[kkaminski]	9/8/2005	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		'<TestFixtureSetUp()> _
		'Public Sub CreateIdentity()
		'	Dim errorMessage As String
		'	Try
		'		_id = _loginManager.Login(System.Security.Principal.WindowsIdentity.GetCurrent)
		'		_errorMessage = String.Empty
		'		_deleteSuccess = False
		'	Catch ex As System.Exception
		'		errorMessage = ex.Message
		'	Finally
		'		Assert.IsNotNull(_id, "Identity creation failed: " & errorMessage)
		'	End Try
		'End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' This method is called immediatly prior to every test method.
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[kkaminski]	9/8/2005	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		<SetUp()> _
		Public Sub ClearErrorMessage()
			_errorMessage = String.Empty
			_deleteSuccess = False
		End Sub

		<Test()> _
		Public Sub _00001_CreateClient()
			Try
				Dim clientDTO As New kCura.edds.DTO.Client
				clientDTO.Name = "Test Client"
				clientDTO.Number = "0001"
				clientDTO.StatusCodeArtifactID = 662				'HACK: Hardcoded
				clientDTO.Keywords = "Keywords"
				clientDTO.Notes = "Notes"
				_fixture.ClientArtifactID = _clientManager.Create(clientDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.ClientArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00002_CreateClientBadInfo()
			Try
				Dim clientDTO As New kCura.edds.DTO.Client
				clientDTO.Name = "Test Client"
				clientDTO.Number = "0001"
				clientDTO.StatusCodeArtifactID = 0				'HACK: Hardcoded
				clientDTO.Keywords = "Keywords"
				clientDTO.Notes = "Notes"
				_fixture.ClientArtifactID = _clientManager.Create(clientDTO, _id)
				Assert.IsTrue(False, "Invalid codeID specified - client create should fail")
			Catch ex As System.Exception
				Assert.IsTrue(True)
			End Try
		End Sub

		<Test()> _
		Public Sub _00003_CreateMatter()
			Dim errorMessage As String
			Try
				Dim matterDTO As New kCura.EDDS.DTO.Matter
				matterDTO.Name = "Test Matter"
				matterDTO.Number = "1"
				matterDTO.ClientArtifactID = _fixture.ClientArtifactID
				matterDTO.StatusCodeArtifactID = 671				'HACK: Hardcoded
				matterDTO.Keywords = "Keywords"
				matterDTO.Notes = "Notes"
				_fixture.MatterArtifactID = _matterManager.Create(matterDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.MatterArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00004_CreateMatterBadInfo()
			Dim errorMessage As String
			Try
				Dim matterDTO As New kCura.EDDS.DTO.Matter
				matterDTO.Name = "Test Matter"
				matterDTO.Number = "1"
				matterDTO.ClientArtifactID = _fixture.ClientArtifactID
				matterDTO.StatusCodeArtifactID = 0			 'HACK: Hardcoded
				matterDTO.Keywords = "Keywords"
				matterDTO.Notes = "Notes"
				_fixture.MatterArtifactID = _matterManager.Create(matterDTO, _id)
				Assert.IsTrue(False, "Invalid codeID specified - matter create should fail")
			Catch ex As System.Exception
				Assert.IsTrue(True)
			End Try
		End Sub

		<Test()> _
		Public Sub _00005_CreateCase()
			Dim caseDTO As New kCura.EDDS.DTO.Case
			Try
				caseDTO = New kCura.EDDS.DTO.Case
				caseDTO.Name = "Test Case"
				caseDTO.EmailAddress = "test@test.com"
				caseDTO.StatusCodeArtifactID = 675				'HACK: Hardcoded
				caseDTO.Keywords = "Keywords"
				caseDTO.Notes = "Notes"
				caseDTO.MatterArtifactID = _fixture.MatterArtifactID
				_fixture.CaseArtifactID = _caseManager.Create(caseDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.CaseArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00010_CreateField()
			Try
				Dim fieldDTO As kCura.edds.DynamicFields.DTO.Field
				fieldDTO = New kCura.edds.DynamicFields.DTO.Field
				fieldDTO.ParentArtifactID = New NullableTypes.NullableInt32(_fixture.CaseArtifactID + 1)
				fieldDTO.ContainerID = fieldDTO.ParentArtifactID
				fieldDTO.AccessControlListID = kCura.EDDS.Config.SystemACLID
				fieldDTO.DisplayName = "Test Field"
				fieldDTO.IsRequired = True
				fieldDTO.Removable = True
				fieldDTO.IsEditable = True
				fieldDTO.AddToFullText = True
				fieldDTO.AddToConceptualText = True
				fieldDTO.IsLinked = True
				fieldDTO.Keywords = "Keywords"
				fieldDTO.Notes = "Notes"
				fieldDTO.FieldTypeID = kCura.EDDS.Types.FieldTypeHelper.FieldType.Code
				fieldDTO.MaxLength = New NullableTypes.NullableInt32(255)
				fieldDTO.FilterType = "TextBox"
				fieldDTO.SecurityLevel = 3
				fieldDTO.FormatString = String.Empty
				fieldDTO.FieldCategoryID = kCura.EDDS.Types.FieldCategory.Generic
				_fixture.CodeFieldArtifactID = _fieldManager.Create(fieldDTO, _id)
				_fixture.CodeFieldCodeTypeID = fieldDTO.CodeArtifactTypeID.Value
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.CodeFieldArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00020_CreateCaseCode()
			Try
				Dim codeDTO As kCura.Code.DTO.Code
				codeDTO = New kCura.Code.DTO.Code
				codeDTO.CodeType = _fixture.CodeFieldCodeTypeID
				codeDTO.IsActive = True
				codeDTO.Name = "Test Code"
				codeDTO.Order = 1
				codeDTO.Keywords = "Keywords"
				codeDTO.Notes = "Notes"
				codeDTO.AccessControlListID = kCura.EDDS.Config.SystemACLID
				codeDTO.ParentArtifactID = New NullableTypes.NullableInt32(_fixture.CaseArtifactID + 1)
				codeDTO.ContainerID = New NullableTypes.NullableInt32(_fixture.CaseArtifactID + 1)
				_fixture.CaseCodeArtifactID = _codeManager.Create(codeDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.CaseCodeArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		 Public Sub _00030_DeleteCaseCode()
			Try
				_deleteSuccess = _codeManager.Delete(_fixture.CaseCodeArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00040_CreateLayout()
			Dim layoutDTO As kCura.EDDS.DTO.Layout
			Try
				layoutDTO = New kCura.edds.DTO.Layout
				layoutDTO.ParentArtifactID = New NullableTypes.NullableInt32(_fixture.CaseArtifactID + 1)
				layoutDTO.AccessControlListID = kCura.EDDS.Config.SystemACLID
				layoutDTO.Name = "Test Layout"
				layoutDTO.Keywords = "Keywords"
				layoutDTO.Notes = "Notes"
				_fixture.LayoutArtifactID = _layoutManager.Create(layoutDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.LayoutArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		 Public Sub _00050_CreateCategory()
			Try
				Dim categoryDTO As New kCura.EDDS.DynamicFields.DTO.Category
				categoryDTO.Title = "Title"
				categoryDTO.Order = 1
				categoryDTO.LayoutArtifactID = _fixture.LayoutArtifactID
				_fixture.CategoryID = _categoryManager.Create(categoryDTO, _id, _fixture.LayoutArtifactID)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.CategoryID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		 Public Sub _00051_AddCategoryToLayout()
			Try
				Dim layoutManager As New kCura.EDDS.Service.LayoutManager
				layoutManager.Update(layoutManager.Read(_fixture.LayoutArtifactID, _id), _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_errorMessage = String.Empty, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00052_AddCodeFieldToLayout()
			Try
				Dim layoutFieldManager As New kCura.EDDS.Service.DynamicFields.LayoutFieldManager
				Dim layoutFieldDTO As New kCura.EDDS.DynamicFields.DTO.LayoutField
				layoutFieldDTO.LayoutArtifactID = _fixture.LayoutArtifactID
				layoutFieldDTO.FieldID = _fixture.CodeFieldArtifactID
				layoutFieldDTO.CategoryID = _fixture.CategoryID
				layoutFieldDTO.Order = 1
				layoutFieldDTO.IsReadOnly = False
				layoutFieldDTO.Row = 1
				layoutFieldDTO.Column = 1
				layoutFieldDTO.Colspan = 1
				layoutFieldDTO.FieldDisplayTypeID = 4
				layoutFieldManager.Create(_id, layoutFieldDTO)
				_layoutManager.Update(_layoutManager.Read(_fixture.LayoutArtifactID, _id), _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_errorMessage = String.Empty, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00060_DeleteLayout()
			Try
				_deleteSuccess = _layoutManager.Delete(_fixture.LayoutArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00070_CreateDocument()
			Try
				Dim caseDTO As kCura.EDDS.DTO.Case = _caseManager.Read(_fixture.CaseArtifactID, _id)
				Dim parentFolderDTO As kCura.EDDS.DTO.Folder = _folderManager.Read(caseDTO.RootFolderID, _id)
				_fixture.DocumentArtifactID = Me.AddDocument(caseDTO, parentFolderDTO, 0)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.DocumentArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00080_CreateSearch()
			Try
				Dim searchDTO As kCura.EDDS.DTO.Search

				searchDTO = New kCura.EDDS.DTO.Search
				searchDTO.AccessControlListID = kCura.EDDS.Config.SystemACLID
				searchDTO.AccessControlListIsInherited = True
				searchDTO.AssociatedArtifactTypeID = kCura.EDDS.ArtifactType.Document
				searchDTO.ContainerID = NullableTypesHelper.ToNullableInt32((_fixture.CaseArtifactID + 1).ToString)
				searchDTO.IsReport = False
				searchDTO.IsVisible = False
				searchDTO.RenderLinks = True
				searchDTO.ParentArtifactID = NullableTypesHelper.ToNullableInt32((_fixture.CaseArtifactID + 1).ToString)
				searchDTO.Type = "Search"

				searchDTO.Name = "SavedSearch"
				searchDTO.ViewCriteria = BuildViewCriteria()
				searchDTO.ViewField = BuildFieldDTOList()
				searchDTO.Concepts = ""
				searchDTO.EditQuery = False
				searchDTO.Query = ""
				searchDTO.TextIdentifier = searchDTO.Name

				_fixture.SearchArtifactID = _searchManager.Create(_id, searchDTO)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.SearchArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00090_RunCriteriaSearch()
			Dim searchDTO As kCura.EDDS.DTO.Search
			Dim caseDTO As kCura.EDDS.DTO.Case = _caseManager.Read(_fixture.CaseArtifactID, _id)
			Dim parentFolderDTO As kCura.EDDS.DTO.Folder = _folderManager.Read(caseDTO.RootFolderID, _id)
			Dim folderList(0) As Int32

			folderList(0) = caseDTO.RootFolderID
			searchDTO = _searchManager.Read(_id, _fixture.SearchArtifactID)
			Dim count As Int32
			Try
				count = _searchManager.Search(_id, folderList, searchDTO, True, "", False, True).Count
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(count <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00091_RunDirectSearch()
			Dim searchDTO As kCura.EDDS.DTO.Search
			Dim caseDTO As kCura.EDDS.DTO.Case = _caseManager.Read(_fixture.CaseArtifactID, _id)
			Dim parentFolderDTO As kCura.EDDS.DTO.Folder = _folderManager.Read(caseDTO.RootFolderID, _id)
			Dim folderList(0) As Int32

			folderList(0) = caseDTO.RootFolderID
			searchDTO = _searchManager.Read(_id, _fixture.SearchArtifactID)
			Dim count As Int32
			Try
				count = _searchManager.Search(_id, folderList, String.Format("ArtifactID = {0}", _fixture.DocumentArtifactID), True, "", False, True).Count
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(count <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00100_DeleteSearch()
			Try
				_deleteSuccess = _searchManager.Delete(_fixture.SearchArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00110_DeleteDocument()
			Try
				_deleteSuccess = _documentManager.Delete(_fixture.DocumentArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00120_CreateProduction()
			Try
				Dim productionDTO As New kCura.EDDS.DTO.Production

				productionDTO.BeginBatesFieldArtifactID = _fixture.CodeFieldArtifactID
				productionDTO.EndBatesFieldArtifactID = _fixture.CodeFieldArtifactID
				productionDTO.ImageShrinkPercent = 100
				productionDTO.BatesStartNumber = 0
				productionDTO.BatesFormat = 0
				productionDTO.SubdirectoryStartNumber = 0
				productionDTO.SubdirectoryMaxFiles = 0
				productionDTO.VolumeStartNumber = 0
				productionDTO.VolumeMaxSize = 0
				productionDTO.ParentArtifactID = New NullableTypes.NullableInt32(_fixture.CaseArtifactID + 1)
				productionDTO.ContainerID = productionDTO.ParentArtifactID
				_fixture.ProductionArtifactID = _productionManager.Create(productionDTO, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_fixture.ProductionArtifactID <> 0, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00130_DeleteProduction()
			Try
				_deleteSuccess = _productionManager.Delete(_fixture.ProductionArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _00140_DeleteCodeField()
			Try
				_deleteSuccess = _fieldManager.Delete(_id, _fixture.CodeFieldArtifactID)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _99997_DeleteCase()
			Try
				_deleteSuccess = _caseManager.Delete(_fixture.CaseArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _99998_DeleteMatter()
			Try
				_deleteSuccess = _matterManager.Delete(_fixture.MatterArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

		<Test()> _
		Public Sub _99999_DeleteClient()
			Try
				_deleteSuccess = _clientManager.Delete(_fixture.ClientArtifactID, _id)
			Catch ex As System.Exception
				_errorMessage = ex.Message
			Finally
				Assert.IsTrue(_deleteSuccess, _errorMessage)
			End Try
		End Sub

#End Region

#Region "Helper Functions"
		Private Function AddDocument(ByVal caseDTO As kCura.EDDS.DTO.Case, ByVal parentFolderDTO As kCura.EDDS.DTO.Folder, ByVal place As Int32) As Int32
			Dim documentDTO As New kCura.EDDS.DTO.Document
			Dim field As kCura.EDDS.DynamicFields.DTO.Field
			Dim unique As String

			unique = ""
			documentDTO.Fields = _fieldManager.RetrieveAllAsArray(_id, parentFolderDTO.ArtifactID)
			For Each field In documentDTO.Fields
				unique += "1"
				Select Case field.FieldTypeID
					Case Types.FieldTypeHelper.FieldType.Boolean
						_fieldManager.SetFieldValue("True", field)
					Case Types.FieldTypeHelper.FieldType.Varchar, Types.FieldTypeHelper.FieldType.Text
						If field.FieldCategoryID = Types.FieldCategory.FullText Then
							_fieldManager.SetFieldValue(String.Format("This is the fulltext for document {0}.", place), field)
							_fixture.FullTextArtifactViewFieldID = field.ArtifactViewFieldID
						Else
							_fieldManager.SetFieldValue(unique, field)
						End If
					Case Else
						_fieldManager.SetFieldValue("", field)
				End Select
			Next
			documentDTO.ParentArtifactID = New NullableTypes.NullableInt32(parentFolderDTO.ArtifactID)
			documentDTO.ContainerID = parentFolderDTO.ContainerID
			documentDTO.AccessControlListIsInherited = True
			documentDTO.AccessControlListID = parentFolderDTO.AccessControlListID
			Return _documentManager.Create(documentDTO, _id)
		End Function

		Private Function BuildViewCriteria() As kCura.View.List.ViewCriteriaDTOList
			Dim viewList As New kCura.View.List.ViewCriteriaDTOList
			Dim viewCriteria As New kCura.View.DTO.ViewCriteria

			viewCriteria.ArtifactViewFieldID = _fixture.FullTextArtifactViewFieldID
			viewCriteria.Operator = "like"
			viewCriteria.Value = "'fulltext'"
			viewList.Add(viewCriteria)
			Return viewList
		End Function

		Private Function BuildFieldDTOList() As kCura.View.List.ViewFieldDTOList
			Dim viewField As New kCura.View.DTO.ViewField
			Dim viewFieldDTOList As New kCura.View.List.ViewFieldDTOList

			viewField.ArtifactViewFieldID = _fixture.FullTextArtifactViewFieldID
			viewField.Order = 0
			viewFieldDTOList.Add(viewField)
			Return viewFieldDTOList
		End Function
#End Region

#Region "Fixture SubClass"
		Private Class Fixture
			Private _clientArtifactID As Int32
			Private _matterArtifactID As Int32
			Private _caseArtifactID As Int32
			Private _categoryID As Int32
			Private _codeFieldArtifactID As Int32
			Private _codeFieldCodeTypeID As Int32
			Private _caseCodeArtifactID As Int32
			Private _layoutArtifactID As Int32
			Private _documentArtifactID As Int32
			Private _searchArtifactID As Int32
			Private _productionArtifactID As Int32
			Private _documentID As Int32
			Private _fullTextArtifactViewFieldID As Int32

			Public Property ClientArtifactID() As Int32
				Get
					Return _clientArtifactID
				End Get
				Set(ByVal value As Int32)
					_clientArtifactID = value
				End Set
			End Property

			Public Property MatterArtifactID() As Int32
				Get
					Return _matterArtifactID
				End Get
				Set(ByVal value As Int32)
					_matterArtifactID = value
				End Set
			End Property

			Public Property CaseArtifactID() As Int32
				Get
					Return _caseArtifactID
				End Get
				Set(ByVal value As Int32)
					_caseArtifactID = value
				End Set
			End Property

			Public Property CategoryID() As Int32
				Get
					Return _categoryID
				End Get
				Set(ByVal value As Int32)
					_categoryID = value
				End Set
			End Property

			Public Property CodeFieldArtifactID() As Int32
				Get
					Return _codeFieldArtifactID
				End Get
				Set(ByVal value As Int32)
					_codeFieldArtifactID = value
				End Set
			End Property

			Public Property CodeFieldCodeTypeID() As Int32
				Get
					Return _codeFieldCodeTypeID
				End Get
				Set(ByVal value As Int32)
					_codeFieldCodeTypeID = value
				End Set
			End Property

			Public Property CaseCodeArtifactID() As Int32
				Get
					Return _caseCodeArtifactID
				End Get
				Set(ByVal value As Int32)
					_caseCodeArtifactID = value
				End Set
			End Property

			Public Property LayoutArtifactID() As Int32
				Get
					Return _layoutArtifactID
				End Get
				Set(ByVal value As Int32)
					_layoutArtifactID = value
				End Set
			End Property

			Public Property DocumentArtifactID() As Int32
				Get
					Return _documentArtifactID
				End Get
				Set(ByVal value As Int32)
					_documentArtifactID = value
				End Set
			End Property

			Public Property SearchArtifactID() As Int32
				Get
					Return _searchArtifactID
				End Get
				Set(ByVal value As Int32)
					_searchArtifactID = value
				End Set
			End Property

			Public Property ProductionArtifactID() As Int32
				Get
					Return _productionArtifactID
				End Get
				Set(ByVal value As Int32)
					_productionArtifactID = value
				End Set
			End Property

			Public Property DocumentID() As Int32
				Get
					Return _documentID
				End Get
				Set(ByVal value As Int32)
					_documentID = value
				End Set
			End Property

			Public Property FullTextArtifactViewFieldID() As Int32
				Get
					Return _fullTextArtifactViewFieldID
				End Get
				Set(ByVal value As Int32)
					_fullTextArtifactViewFieldID = value
				End Set
			End Property
		End Class
#End Region

	End Class
End Namespace
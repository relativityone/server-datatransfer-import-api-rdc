Imports System.Collections.Generic
Imports kCura.EDDS.WebAPI.DocumentManagerBase
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerFieldQuery
        Inherits KeplerManager
        Implements IFieldQuery

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

		Public Function RetrieveAllMappable(caseContextArtifactID As Integer, artifactTypeID As Integer) As DataSet Implements IFieldQuery.RetrieveAllMappable
            Return Execute(Async Function(s)
							   Using service As Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService = s.CreateProxyInstance(Of Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService)
								   Return Await service.RetrieveAllMappableAsync(caseContextArtifactID, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
							   End Using
						   End Function)
		End Function

		Public Function RetrievePotentialBeginBatesFields(caseContextArtifactID As Integer) As DataSet Implements IFieldQuery.RetrievePotentialBeginBatesFields
			Return Execute(Async Function(s)
							   Using service As Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService = s.CreateProxyInstance(Of Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService)
								   Return Await service.RetrievePotentialBeginBatesFieldsAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
							   End Using
						   End Function)
		End Function

		Public Function IsFieldIndexed(caseContextArtifactID As Integer, fieldArtifactID As Integer) As Boolean Implements IFieldQuery.IsFieldIndexed
			Return Execute(Async Function(s)
							   Using service As Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService = s.CreateProxyInstance(Of Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService)
								   Return Await service.IsFieldIndexedAsync(caseContextArtifactID, fieldArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
							   End Using
						   End Function)
        End Function

        Public Function RetrieveAllAsDocumentFieldCollection(caseContextArtifactID As Integer, artifactTypeID As Integer) As DocumentFieldCollection Implements IFieldQuery.RetrieveAllAsDocumentFieldCollection
            Dim retval As New DocumentFieldCollection
            For Each fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.RetrieveAllAsArray(caseContextArtifactID, artifactTypeID)
                With (fieldDTO)
                    retval.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .AssociativeArtifactTypeID, .UseUnicodeEncoding, .ImportBehavior, .Guids, .EnableDataGrid))
                End With
            Next
            Return retval
        End Function

        Public Function RetrieveAllAsArray(caseContextArtifactID As Integer, artifactTypeID As Integer, Optional includeUnmappable As Boolean = False) As Field() Implements IFieldQuery.RetrieveAllAsArray
            Dim dv As New SqlDataView(RetrieveAllMappable(caseContextArtifactID, artifactTypeID))
			Dim fields As New System.Collections.ArrayList
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim unmappableFields As New System.Collections.Specialized.StringCollection
			Dim unmappableFieldCategories As New System.Collections.ArrayList
			If Not includeUnmappable Then
				unmappableFieldCategories.AddRange(New Global.Relativity.DataExchange.Service.FieldCategory() {Global.Relativity.DataExchange.Service.FieldCategory.Reflected, Global.Relativity.DataExchange.Service.FieldCategory.Batch, Global.Relativity.DataExchange.Service.FieldCategory.MultiReflected, Global.Relativity.DataExchange.Service.FieldCategory.FileInfo, Global.Relativity.DataExchange.Service.FieldCategory.AutoCreate, Global.Relativity.DataExchange.Service.FieldCategory.FileSize, Global.Relativity.DataExchange.Service.FieldCategory.ProductionMarker, Global.Relativity.DataExchange.Service.FieldCategory.MarkupSetMarker})
			End If
			Dim i As Int32
			For i = 0 To dv.Count - 1
				field = New kCura.EDDS.WebAPI.DocumentManagerBase.Field
				If Not ( _
				 unmappableFieldCategories.Contains(CType(dv(i)("FieldCategoryID"), Global.Relativity.DataExchange.Service.FieldCategory)) _
				 OrElse _
				 unmappableFields.Contains(dv(i)("DisplayName").ToString) _
				) Then
					If Not (CType(dv(i)("FieldCategoryID"), Global.Relativity.DataExchange.Service.FieldCategory) = Global.Relativity.DataExchange.Service.FieldCategory.FullText AndAlso artifactTypeID <> ArtifactType.Document) Then
						Dim guidsString As String = dv(i)("ArtifactGuids").ToString()
						Dim guids As New List(Of Guid)
						If (Not String.IsNullOrEmpty(guidsString)) Then
							Dim guidStringArray As String() = guidsString.Split(CChar(";"))
							For Each guidString As String In guidStringArray
								guids.Add(New Guid(guidString.Trim()))
							Next
						End If
						With field
							.ArtifactID = CType(dv(i)("ArtifactID"), Int32)
							.ArtifactViewFieldID = CType(dv(i)("ArtifactViewFieldID"), Int32)
							.CodeTypeID = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("CodeTypeID"))
							.DisplayName = CType(dv(i)("DisplayName"), String)
							.FieldCategoryID = CType(dv(i)("FieldCategoryID"), Int32)
							.FieldCategory = CType(dv(i)("FieldCategoryID"), kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
							.FieldType = CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.FieldType), CType(dv(i)("FieldTypeID"), Global.Relativity.DataExchange.Service.FieldType).ToString), kCura.EDDS.WebAPI.DocumentManagerBase.FieldType)
							.FieldTypeID = CInt(dv(i)("FieldTypeID"))
							.IsEditable = CType(dv(i)("IsEditable"), Boolean)
							.IsRequired = CType(dv(i)("IsRequired"), Boolean)
							.MaxLength = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("FieldLength"))
							.IsRemovable = CType(dv(i)("IsRemovable"), Boolean)
							.IsVisible = CType(dv(i)("IsVisible"), Boolean)
							.UseUnicodeEncoding = CType(dv(i)("UseUnicodeEncoding"), Boolean)
							.AllowHtml = CType(dv(i)("AllowHTML"), Boolean)
							.AssociativeArtifactTypeID = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("AssociativeArtifactTypeID"))
							.ImportBehavior = Me.ConvertImportBehaviorEnum(NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dv(i)("ImportBehavior")))
							.EnableDataGrid = CBool(dv(i)("EnableDataGrid"))
							.Guids = guids.ToArray()
						End With
						If field.FieldType = EDDS.WebAPI.DocumentManagerBase.FieldType.Object OrElse field.FieldType = EDDS.WebAPI.DocumentManagerBase.FieldType.Objects OrElse field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.MultiReflected OrElse field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Reflected Then
							If field.AssociativeArtifactTypeID.HasValue AndAlso ArtifactTypeHelper.IsDynamic(field.AssociativeArtifactTypeID.Value) Then fields.Add(field)
						Else
							fields.Add(field)
						End If
					End If
				End If

			Next
			Return DirectCast(fields.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
        End Function

        Private Function ConvertImportBehaviorEnum(ByVal input As Int32?) As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice?
            If Not input.HasValue Then Return Nothing
            Dim ibc As Global.Relativity.DataExchange.Service.ImportBehaviorChoice = CType(input, Global.Relativity.DataExchange.Service.ImportBehaviorChoice)
            Return CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice), ibc.ToString), kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice)
        End Function
    End Class
End Namespace

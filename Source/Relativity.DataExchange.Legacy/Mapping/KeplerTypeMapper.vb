Imports AutoMapper
Imports RelativityDataTransferLegacySDK = Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models
Imports RelativityDataExchange = Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Mapping

    Public Class KeplerTypeMapper
        Implements ITypeMapper

        Private ReadOnly _mapper As IMapper

        Public Sub New()
            Dim config As MapperConfiguration = New MapperConfiguration(AddressOf ConfigureMapper)
            _mapper = config.CreateMapper()
        End Sub

        Public Function Map(Of T)(source As Object) As T Implements ITypeMapper.Map
            Return _mapper.Map(Of T)(source)
        End Function

        Public Function Map(source As Object, sourceType As Type, destinationType As Type) As Object
            Return _mapper.Map(source, sourceType, destinationType)
        End Function

        Private Sub ConfigureMapper(config As IMapperConfigurationExpression)
            ConfigureParameterTypeMappings(config)
            ConfigureReturnTypeMappings(config)
        End Sub

        Private Sub ConfigureParameterTypeMappings(config As IMapperConfigurationExpression)
            config.CreateMap(Of kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics, RelativityDataTransferLegacySDK.ImageImportStatistics)
            config.CreateMap(Of kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics, RelativityDataTransferLegacySDK.ObjectImportStatistics)
            config.CreateMap(Of kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics, RelativityDataTransferLegacySDK.ExportStatistics)
            config.CreateMap(Of kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, RelativityDataTransferLegacySDK.ImageLoadInfo)
            config.CreateMap(Of kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, RelativityDataTransferLegacySDK.NativeLoadInfo)
            config.CreateMap(Of kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, RelativityDataTransferLegacySDK.ObjectLoadInfo)
            config.CreateMap(Of kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange, RelativityDataTransferLegacySDK.LoadRange)
            config.CreateMap(Of kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo, RelativityDataTransferLegacySDK.FieldInfo)
            config.CreateMap(Of kCura.EDDS.WebAPI.CodeManagerBase.Code, RelativityDataTransferLegacySDK.Code)
            config.CreateMap(Of kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut, RelativityDataTransferLegacySDK.KeyboardShortcut)
        End Sub

        Private Sub ConfigureReturnTypeMappings(config As IMapperConfigurationExpression)
            config.CreateMap(Of RelativityDataTransferLegacySDK.MassImportResults, kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)
            config.CreateMap(Of RelativityDataTransferLegacySDK.SoapExceptionDetail, kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
            config.CreateMap(Of RelativityDataTransferLegacySDK.ErrorFileKey, RelativityDataExchange.ErrorFileKey)
            config.CreateMap(Of RelativityDataTransferLegacySDK.CaseInfo, RelativityDataExchange.CaseInfo)
            config.CreateMap(Of RelativityDataTransferLegacySDK.ChoiceInfo, RelativityDataExchange.ChoiceInfo)
            config.CreateMap(Of RelativityDataTransferLegacySDK.Code, kCura.EDDS.WebAPI.CodeManagerBase.Code)
            config.CreateMap(Of RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut)
            config.CreateMap(Of RelativityDataTransferLegacySDK.InitializationResults, kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults)
            config.CreateMap(Of RelativityDataTransferLegacySDK.Field, kCura.EDDS.WebAPI.FieldManagerBase.Field)
            config.CreateMap(Of RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.FieldManagerBase.KeyboardShortcut)
            config.CreateMap(Of RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.FieldManagerBase.RelationalFieldPane)
            config.CreateMap(Of RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.FieldManagerBase.ObjectsFieldParameters)
            config.CreateMap(Of RelativityDataTransferLegacySDK.Field, kCura.EDDS.WebAPI.DocumentManagerBase.Field)
            config.CreateMap(Of RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.DocumentManagerBase.KeyboardShortcut)
            config.CreateMap(Of RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.DocumentManagerBase.RelationalFieldPane)
            config.CreateMap(Of RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.DocumentManagerBase.ObjectsFieldParameters)
            config.CreateMap(Of RelativityDataTransferLegacySDK.Folder, kCura.EDDS.WebAPI.FolderManagerBase.Folder)
            config.CreateMap(Of RelativityDataTransferLegacySDK.ProductionInfo, kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo)
        End Sub
    End Class
End NameSpace
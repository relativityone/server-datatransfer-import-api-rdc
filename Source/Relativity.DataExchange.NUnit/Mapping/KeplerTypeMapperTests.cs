// -----------------------------------------------------------------------------------------------------
// <copyright file="KeplerTypeMapperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="KeplerTypeMapperTests"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Mapping
{
	using System;
	using System.Collections;

	using AutoFixture;
    using AutoMapper;
	using FluentAssertions;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Mapping;
	using RelativityDataExchange = Relativity.DataExchange.Service;
	using RelativityDataTransferLegacySDK = Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;

    [TestFixture]
	public class KeplerTypeMapperTests
	{
		private IMapper mapper;
		private Fixture fixture;

		[OneTimeSetUp]
        public void OneTimeSetUp()
		{
			MapperConfiguration config = new MapperConfiguration(this.ConfigureMapper);
			this.mapper = config.CreateMapper();
			this.fixture = new Fixture();
		}

        [TestCaseSource(typeof(MappingCases))]
        public void MapWithDefaultValuesTest<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
	        where TSource : new()
        {
	        // arrange
	        TSource source = new TSource();
	        var expected = this.mapper.Map<TDestination>(source);

	        // act
	        var result = mapFunc(source);

	        // assert
	        result.Should().BeEquivalentTo(expected);
        }

		[TestCaseSource(typeof(MappingCases))]
        public void MapWithRandomValuesTest<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
        {
	        // arrange
	        TSource source = this.fixture.Create<TSource>();
	        var expected = this.mapper.Map<TDestination>(source);

	        // act
	        var result = mapFunc(source);

	        // assert
	        result.Should().BeEquivalentTo(expected);
		}

        private void ConfigureMapper(IMapperConfigurationExpression config)
        {
	        config.AllowNullCollections = true;
	        this.ConfigureParameterTypeMappings(config);
            this.ConfigureReturnTypeMappings(config);
        }

        private void ConfigureParameterTypeMappings(IMapperConfigurationExpression config)
        {
	        config.CreateMap<kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics, RelativityDataTransferLegacySDK.ImageImportStatistics>();
            config.CreateMap<kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics, RelativityDataTransferLegacySDK.ObjectImportStatistics>();
            config.CreateMap<kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics, RelativityDataTransferLegacySDK.ExportStatistics>();
            config.CreateMap<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, RelativityDataTransferLegacySDK.ImageLoadInfo>();
            config.CreateMap<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, RelativityDataTransferLegacySDK.NativeLoadInfo>();

            config.CreateMap<kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, RelativityDataTransferLegacySDK.ObjectLoadInfo>();
            config.CreateMap<kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange, RelativityDataTransferLegacySDK.LoadRange>();
            config.CreateMap<kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo, RelativityDataTransferLegacySDK.FieldInfo>();
            config.CreateMap<kCura.EDDS.WebAPI.CodeManagerBase.Code, RelativityDataTransferLegacySDK.Code>();
            config.CreateMap<kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut, RelativityDataTransferLegacySDK.KeyboardShortcut>();
        }

        private void ConfigureReturnTypeMappings(IMapperConfigurationExpression config)
        {
            config.CreateMap<RelativityDataTransferLegacySDK.MassImportResults, kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults>();
            config.CreateMap<RelativityDataTransferLegacySDK.SoapExceptionDetail, kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail>();
            config.CreateMap<RelativityDataTransferLegacySDK.ErrorFileKey, RelativityDataExchange.ErrorFileKey>();
            config.CreateMap<RelativityDataTransferLegacySDK.CaseInfo, RelativityDataExchange.CaseInfo>();
            config.CreateMap<RelativityDataTransferLegacySDK.ChoiceInfo, RelativityDataExchange.ChoiceInfo>();

            config.CreateMap<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut>();
            config.CreateMap<RelativityDataTransferLegacySDK.InitializationResults, kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults>();
            config.CreateMap<RelativityDataTransferLegacySDK.Field, kCura.EDDS.WebAPI.FieldManagerBase.Field>();
            config.CreateMap<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.FieldManagerBase.KeyboardShortcut>();
            config.CreateMap<RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.FieldManagerBase.RelationalFieldPane>();

            config.CreateMap<RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.FieldManagerBase.ObjectsFieldParameters>();
            config.CreateMap<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.DocumentManagerBase.KeyboardShortcut>();
            config.CreateMap<RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.DocumentManagerBase.RelationalFieldPane>();
            config.CreateMap<RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.DocumentManagerBase.ObjectsFieldParameters>();
            config.CreateMap<RelativityDataTransferLegacySDK.Folder, kCura.EDDS.WebAPI.FolderManagerBase.Folder>();

            config.CreateMap<RelativityDataTransferLegacySDK.ProductionInfo, kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo>();
        }

        public class MappingCases : IEnumerable
		{
			public IEnumerator GetEnumerator()
			{
				yield return new object[] { (Func<kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics, RelativityDataTransferLegacySDK.ImageImportStatistics>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics, RelativityDataTransferLegacySDK.ObjectImportStatistics>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics, RelativityDataTransferLegacySDK.ExportStatistics>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, RelativityDataTransferLegacySDK.ImageLoadInfo>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, RelativityDataTransferLegacySDK.NativeLoadInfo>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, RelativityDataTransferLegacySDK.ObjectLoadInfo>)KeplerTypeMapper.Map };

				yield return new object[] { (Func<kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange, RelativityDataTransferLegacySDK.LoadRange>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo, RelativityDataTransferLegacySDK.FieldInfo>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.CodeManagerBase.Code, RelativityDataTransferLegacySDK.Code>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut, RelativityDataTransferLegacySDK.KeyboardShortcut>)KeplerTypeMapper.Map };

				yield return new object[] { (Func<RelativityDataTransferLegacySDK.MassImportResults, kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.SoapExceptionDetail, kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.ErrorFileKey, RelativityDataExchange.ErrorFileKey>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.CaseInfo, RelativityDataExchange.CaseInfo>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.ChoiceInfo, RelativityDataExchange.ChoiceInfo>)KeplerTypeMapper.Map };

				yield return new object[] { (Func<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut>)KeplerTypeMapper.MapToCodeManagerBaseKeyboardShortcut };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.InitializationResults, kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults>)KeplerTypeMapper.Map };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.Field, kCura.EDDS.WebAPI.FieldManagerBase.Field>)KeplerTypeMapper.MapToFieldManagerBaseField };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.FieldManagerBase.KeyboardShortcut>)KeplerTypeMapper.MapToFieldManagerBaseKeyboardShortcut };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.FieldManagerBase.RelationalFieldPane>)KeplerTypeMapper.MapToFieldManagerBaseRelationalFieldPane };

				yield return new object[] { (Func<RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.FieldManagerBase.ObjectsFieldParameters>)KeplerTypeMapper.MapToFieldManagerBaseObjectsFieldParameters };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.KeyboardShortcut, kCura.EDDS.WebAPI.DocumentManagerBase.KeyboardShortcut>)KeplerTypeMapper.MapToDocumentManagerBaseKeyboardShortcut };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.RelationalFieldPane, kCura.EDDS.WebAPI.DocumentManagerBase.RelationalFieldPane>)KeplerTypeMapper.MapToDocumentManagerBaseRelationalFieldPane };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.ObjectsFieldParameters, kCura.EDDS.WebAPI.DocumentManagerBase.ObjectsFieldParameters>)KeplerTypeMapper.MapToDocumentManagerBaseObjectsFieldParameters };
				yield return new object[] { (Func<RelativityDataTransferLegacySDK.Folder, kCura.EDDS.WebAPI.FolderManagerBase.Folder>)KeplerTypeMapper.Map };

				yield return new object[] { (Func<RelativityDataTransferLegacySDK.ProductionInfo, kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo>)KeplerTypeMapper.Map };
			}
		}
	}
}
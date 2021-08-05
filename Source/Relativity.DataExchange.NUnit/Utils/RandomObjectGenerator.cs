// <copyright file="RandomObjectGenerator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Utils
{
    using System;
    using System.Collections.Generic;
    using AutoBogus;
	using Bogus;

    public class RandomObjectGenerator
	{
		private readonly IDictionary<Type, Func<object>> generators = new Dictionary<Type, Func<object>>();

		public RandomObjectGenerator()
		{
			this.InitializeWebApiModelsGenerators();
			this.InitializeKeplerModelsGenerators();
		}

		public object Generate(Type type)
		{
			if (!this.generators.ContainsKey(type))
			{
				return null;
			}

			return this.generators[type]();
		}

		private void InitializeWebApiModelsGenerators()
		{
			this.generators.Add(typeof(kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics), () => AutoFaker.Generate<kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics), () => AutoFaker.Generate<kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics), () => AutoFaker.Generate<kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo), () => AutoFaker.Generate<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo), () => AutoFaker.Generate<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo), () => AutoFaker.Generate<kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange), () => AutoFaker.Generate<kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo), () => AutoFaker.Generate<kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.CodeManagerBase.Code), () => AutoFaker.Generate<kCura.EDDS.WebAPI.CodeManagerBase.Code>());
			this.generators.Add(typeof(kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut), () => AutoFaker.Generate<kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut>());
		}

		private void InitializeKeplerModelsGenerators()
		{
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.MassImportResults), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.MassImportResults>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ErrorFileKey), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ErrorFileKey>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.CaseInfo), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.CaseInfo>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Code), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Code>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ChoiceInfo), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ChoiceInfo>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ChoiceInfo[]), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ChoiceInfo[]>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.KeyboardShortcut), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.KeyboardShortcut>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.InitializationResults), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.InitializationResults>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Field), () => this.GetFieldFaker().Generate());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Field[]), () => this.GetFieldFaker().Generate(3).ToArray());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Folder), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Folder>());
			this.generators.Add(typeof(Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ProductionInfo), () => AutoFaker.Generate<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ProductionInfo>());
		}

		private Faker<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Field> GetFieldFaker()
		{
			// ignore ImportBehaviorChoice.ChoiceFieldIgnoreDuplicates because it is missing in kCura.EDDS.WebAPI.BulkImportManagerBase.ImportBehaviorChoice
			var fieldFaker = new AutoFaker<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Field>().RuleFor(
				field => field.ImportBehavior,
				faker => faker.PickRandom(
					new List<Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ImportBehaviorChoice?>
						{
							Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ImportBehaviorChoice.LeaveBlankValuesUnchanged,
							Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier,
							Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.ImportBehaviorChoice.ObjectFieldContainsArtifactId,
							null
						}));
			return fieldFaker;
		}
	}
}

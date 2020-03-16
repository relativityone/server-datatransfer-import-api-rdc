// <copyright file="IImportApiSetup.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	public interface IImportApiSetup<in TSettings>
		where TSettings : ImportSettingsBase
	{
		void SetUpImportApi(Func<ImportAPI> importApiFactory, TSettings settings);

		void SetUpImportApi(Func<ImportAPI> importApiFactory, ISettingsBuilder<TSettings> settingsBuilder);

		void SetUpImportApi(IntegrationTestParameters parameters, ISettingsBuilder<TSettings> settingsBuilder);
	}
}

// <copyright file="ImportApiSetupExtensions.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration.JobExecutionContext
{
	using System;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.ImportDataSource;

	public static class ImportApiSetupExtensions
	{
		public static void InitializeImportApiWithUserAndPassword<TSettings>(this IImportApiSetup<TSettings> setup, IntegrationTestParameters testParameters, TSettings settings)
			where TSettings : ImportSettingsBase
		{
			setup = setup ?? throw new ArgumentNullException(nameof(setup));
			setup.SetUpImportApi(() => CreateImportApiWithUserCredentials(testParameters), settings);
		}

		public static void InitializeImportApiWithUserAndPassword<TSettings>(this IImportApiSetup<TSettings> setup, IntegrationTestParameters testParameters, ISettingsBuilder<TSettings> settingsBuilder)
			where TSettings : ImportSettingsBase
		{
			setup = setup ?? throw new ArgumentNullException(nameof(setup));
			setup.SetUpImportApi(() => CreateImportApiWithUserCredentials(testParameters), settingsBuilder);
		}

		public static void InitializeImportApiWithIntegratedAuthentication<TSettings>(this IImportApiSetup<TSettings> setup, IntegrationTestParameters testParameters, TSettings settings)
			where TSettings : ImportSettingsBase
		{
			setup = setup ?? throw new ArgumentNullException(nameof(setup));
			setup.SetUpImportApi(() => CreateImportApiWithIntegratedAuthentication(testParameters), settings);
		}

		private static ImportAPI CreateImportApiWithUserCredentials(IntegrationTestParameters testParameters)
		{
			return new ImportAPI(
				testParameters.RelativityUserName,
				testParameters.RelativityPassword,
				testParameters.RelativityWebApiUrl.ToString());
		}

		private static ImportAPI CreateImportApiWithIntegratedAuthentication(IntegrationTestParameters testParameters)
		{
			return new ImportAPI(testParameters.RelativityWebApiUrl.ToString());
		}
	}
}

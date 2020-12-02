﻿// <copyright file="ImportApiSetupExtensions.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

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

		private static ImportAPI CreateImportApiWithUserCredentials(IntegrationTestParameters testParameters)
		{
			return new ImportAPI(
				testParameters.RelativityUserName,
				testParameters.RelativityPassword,
				testParameters.RelativityWebApiUrl.ToString());
		}
	}
}

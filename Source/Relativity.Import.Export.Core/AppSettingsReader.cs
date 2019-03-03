// ----------------------------------------------------------------------------
// <copyright file="AppSettingsReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;

	/// <summary>
	/// Defines a static method to retrieve application settings.
	/// </summary>
	internal static class AppSettingsReader
	{
		/// <summary>
		/// Gets or sets the Registry sub-key name.
		/// </summary>
		/// <value>
		/// The sub-key name.
		/// </value>
		/// <remarks>
		/// This is provided strictly for testing purposes.
		/// </remarks>
		internal static string RegistrySubKeyName { get; set; } = @"Software\kCura\Relativity";

		/// <summary>
		/// Reads all application settings from standard .NET settings and the Windows Registry.
		/// </summary>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		public static IAppSettings Read()
		{
			// For backwards compatibility, support all legacy sections.
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			ReadSectionValues("kCura.Windows.Process", dictionary);
			ReadSectionValues("kCura.Config", dictionary);
			ReadSectionValues("kCura.Utility", dictionary);
			ReadSectionValues("kCura.WinEDDS", dictionary);
			ReadSectionValues("Relativity.Import.Export", dictionary);
			IAppSettings settings = new AppDotNetSettings();
			Refresh(settings);
			return settings;
		}

		/// <summary>
		/// Refreshes the specified application settings.
		/// </summary>
		/// <param name="settings">
		/// The application settings to refresh.
		/// </param>
		public static void Refresh(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			// For backwards compatibility, support all legacy sections.
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			ReadSectionValues("kCura.Windows.Process", dictionary);
			ReadSectionValues("kCura.Config", dictionary);
			ReadSectionValues("kCura.Utility", dictionary);
			ReadSectionValues("kCura.WinEDDS", dictionary);
			ReadSectionValues("Relativity.Import.Export", dictionary);
			settings.CreateErrorForInvalidDate = dictionary.GetBooleanValue(
				AppSettingsConstants.CreateErrorForInvalidDateKey,
				AppSettingsConstants.CreateErrorForInvalidDateDefaultValue);
			settings.DisableThrowOnIllegalCharacters = dictionary.GetBooleanValue(
				AppSettingsConstants.DisableThrowOnIllegalCharactersKey,
				AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue);
			settings.ExportErrorNumberOfRetries = dictionary.GetInt32Value(
				AppSettingsConstants.ExportErrorNumberOfRetriesKey,
				AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue,
				AppSettingsConstants.ExportErrorNumberOfRetriesMinValue);
			settings.ExportErrorWaitTimeInSeconds = dictionary.GetInt32Value(
				AppSettingsConstants.ExportErrorWaitTimeInSecondsKey,
				AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue,
				AppSettingsConstants.ExportErrorWaitTimeInSecondsMinValue);
			settings.ForceFolderPreview = dictionary.GetBooleanValue(
				AppSettingsConstants.ForceFolderPreviewKey,
				AppSettingsConstants.ForceFolderPreviewDefaultValue);
			settings.IoErrorNumberOfRetries = dictionary.GetInt32Value(
				AppSettingsConstants.IoErrorNumberOfRetriesKey,
				AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue,
				AppSettingsConstants.IoErrorNumberOfRetriesMinValue);
			settings.IoErrorWaitTimeInSeconds = dictionary.GetInt32Value(
				AppSettingsConstants.IoErrorWaitTimeInSecondsKey,
				AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue,
				AppSettingsConstants.IoErrorWaitTimeInSecondsMinValue);
			settings.LogAllEvents = dictionary.GetBooleanValue(
				AppSettingsConstants.LogAllEventsKey,
				AppSettingsConstants.LogAllEventsDefaultValue);
			settings.MaxNumberOfFileExportTasks = dictionary.GetInt32Value(
				AppSettingsConstants.MaxNumberOfFileExportTasksKey,
				AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue,
				AppSettingsConstants.MaxNumberOfFileExportTasksMinValue);
			settings.MaximumFilesForTapiBridge = dictionary.GetInt32Value(
				AppSettingsConstants.MaximumFilesForTapiBridgeKey,
				AppSettingsConstants.MaximumFilesForTapiBridgeDefaultValue,
				AppSettingsConstants.MaximumFilesForTapiBridgeMinValue);
			settings.TapiBridgeExportTransferWaitingTimeInSeconds = dictionary.GetInt32Value(
				AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsKey,
				AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue,
				AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsMinValue);

			// This complexity is due to 3 possible values including the Windows Registry.
			AppDotNetSettings dotNetSettings = settings as AppDotNetSettings;
			Uri webApiServiceUrl = dictionary.GetUriValue(AppSettingsConstants.WebApiServiceUrlKey, null);
			if (dotNetSettings != null)
			{
				dotNetSettings.RefreshWebApiServiceUrl(webApiServiceUrl);
			}
			else
			{
				settings.WebApiServiceUrl = webApiServiceUrl;
			}
		}

		/// <summary>
		/// Sets the registry key value.
		/// </summary>
		/// <param name="keyName">
		/// The Registry key name.
		/// </param>
		/// <param name="value">
		/// The setting value.
		/// </param>
		internal static void SetRegistryKeyValue(string keyName, string value)
		{
			const bool Write = true;
			using (Microsoft.Win32.RegistryKey key = GetRegistryKey(Write))
			{
				key.SetValue(keyName, value);
			}
		}

		/// <summary>
		/// Gets the registry key.
		/// </summary>
		/// <param name="write">
		/// Specify whether write access is required.
		/// </param>
		/// <returns>
		/// The <see cref="Microsoft.Win32.RegistryKey"/> instance.
		/// </returns>
		internal static Microsoft.Win32.RegistryKey GetRegistryKey(bool write)
		{
			Microsoft.Win32.RegistryKey key =
				Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistrySubKeyName, write);
			if (key != null)
			{
				return key;
			}

			Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistrySubKeyName);
			key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistrySubKeyName, write);
			return key;
		}

		/// <summary>
		/// Gets the registry key value.
		/// </summary>
		/// <param name="keyName">
		/// The Registry key name.
		/// </param>
		/// <returns>
		/// The key value.
		/// </returns>
		internal static string GetRegistryKeyValue(string keyName)
		{
			const bool Write = false;
			using (Microsoft.Win32.RegistryKey key = GetRegistryKey(Write))
			{
				string value = key.GetValue(keyName, string.Empty) as string;
				return value;
			}
		}

		/// <summary>
		/// Validates to ensure the URI format is proper.
		/// </summary>
		/// <param name="value">
		/// The string representation of the URI.
		/// </param>
		/// <returns>
		/// The validated URI string.
		/// </returns>
		internal static string ValidateUriFormat(string value)
		{
			if (!string.IsNullOrEmpty(value) && !value.Trim().EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Trim() + "/";
			}

			Uri result;
			if (Uri.TryCreate(value, UriKind.Absolute, out result))
			{
				return value;
			}

			return string.Empty;
		}

		/// <summary>
		/// Reads the app config section name/value pairs and add to the dictionary.
		/// </summary>
		/// <param name="sectionName">
		/// Name of the section.
		/// </param>
		/// <param name="settings">
		/// The settings dictionary where all section values are added.
		/// </param>
		private static void ReadSectionValues(string sectionName, IDictionary<string, object> settings)
		{
			try
			{
				System.Collections.Hashtable section =
					ConfigurationManager.GetSection(sectionName) as System.Collections.Hashtable;
				if (section == null)
				{
					return;
				}

				Dictionary<string, object> sectionDictionary = section.Cast<System.Collections.DictionaryEntry>()
					.ToDictionary(n => n.Key.ToString(), n => n.Value);
				foreach (var key in sectionDictionary.Keys)
				{
					settings[key] = sectionDictionary[key];
				}
			}
			catch (ConfigurationErrorsException)
			{
				// Due to legacy concerns, never allow configuration errors to throw.
			}
		}
	}
}
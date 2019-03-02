// ----------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;

	/// <summary>
	/// Represents a class object that provides thread-safe application settings.
	/// </summary>
	/// <remarks>
	/// Ensure that <see cref="AppSettingsDto"/> is updated when adding new settings.
	/// </remarks>
	[Serializable]
	public sealed class AppSettings : IAppSettings
	{
		/// <summary>
		/// The concurrent dictionary that caches all settings.
		/// </summary>
		private readonly ConcurrentDictionary<string, object> cachedSettings = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettings"/> class.
		/// </summary>
		public AppSettings()
		{
			((IAppSettings)this).Refresh();
		}

		/// <summary>
		/// Gets or sets a value indicating whether to create an error for an invalid date.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create an error; otherwise, <see langword="false" />.
		/// </value>
		public static bool CreateErrorForInvalidDate
		{
			get => Instance.CreateErrorForInvalidDate;
			set => Instance.CreateErrorForInvalidDate = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable throwing exceptions when illegal characters are found within a path.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable throwing an exception; otherwise, <see langword="false" />.
		/// </value>
		public static bool DisableThrowOnIllegalCharacters
		{
			get => Instance.DisableThrowOnIllegalCharacters;
			set => Instance.DisableThrowOnIllegalCharacters = value;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		public static int ExportErrorNumberOfRetries
		{
			get => Instance.ExportErrorNumberOfRetries;
			set => Instance.ExportErrorNumberOfRetries = value;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public static int ExportErrorWaitTimeInSeconds
		{
			get => Instance.ExportErrorWaitTimeInSeconds;
			set => Instance.ExportErrorWaitTimeInSeconds = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force a folder preview.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force a folder preview; otherwise, <see langword="false" />.
		/// </value>
		public static bool ForceFolderPreview
		{
			get => Instance.ForceFolderPreview;
			set => Instance.ForceFolderPreview = value;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		public static int IoErrorNumberOfRetries
		{
			get => Instance.IoErrorNumberOfRetries;
			set => Instance.IoErrorNumberOfRetries = value;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public static int IoErrorWaitTimeInSeconds
		{
			get => Instance.IoErrorWaitTimeInSeconds;
			set => Instance.IoErrorWaitTimeInSeconds = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to log all the I/O events.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to log all the I/O events; otherwise, <see langword="false" />.
		/// </value>
		public static bool LogAllEvents
		{
			get => Instance.LogAllEvents;
			set => Instance.LogAllEvents = value;
		}

		/// <summary>
		/// Gets or sets the maximum number of files for each Transfer API bridge instance.
		/// </summary>
		/// <value>
		/// The maximum number of files.
		/// </value>
		public static int MaximumFilesForTapiBridge
		{
			get => Instance.MaximumFilesForTapiBridge;
			set => Instance.MaximumFilesForTapiBridge = value;
		}

		/// <summary>
		/// Gets or sets the maximum number of file export tasks.
		/// </summary>
		/// <value>
		/// The maximum number of tasks.
		/// </value>
		public static int MaxNumberOfFileExportTasks
		{
			get => Instance.MaxNumberOfFileExportTasks;
			set => Instance.MaxNumberOfFileExportTasks = value;
		}

		/// <summary>
		/// Gets or sets the list of artifacts to use for object field mapping instead of the name field.
		/// </summary>
		/// <value>
		/// The list of artifacts.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2227:CollectionPropertiesShouldBeReadOnly",
			Justification = "This is required for backwards compatibility.")]
		public static IList<int> ObjectFieldIdListContainsArtifactId
		{
			get => Instance.ObjectFieldIdListContainsArtifactId;
			set => Instance.ObjectFieldIdListContainsArtifactId = value;
		}

		/// <summary>
		/// Gets or sets the programmatic Relativity Web API service URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public static Uri ProgrammaticWebApiServiceUrl
		{
			get => Instance.ProgrammaticWebApiServiceUrl;
			set => Instance.ProgrammaticWebApiServiceUrl = value;
		}

		/// <summary>
		/// Gets or sets the time, in seconds, that a Transfer API bridge waits before releasing the wait handle.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public static int TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get => Instance.TapiBridgeExportTransferWaitingTimeInSeconds;
			set => Instance.TapiBridgeExportTransferWaitingTimeInSeconds = value;
		}

		/// <summary>
		/// Gets or sets the Relativity Web API service URL. This will always return <see cref="ProgrammaticWebApiServiceUrl"/> and then this value. If none are defined, a final check is made with the Windows Registry to determine if it has been set of the RDC.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public static Uri WebApiServiceUrl
		{
			get => Instance.WebApiServiceUrl;
			set => Instance.WebApiServiceUrl = value;
		}

		/// <inheritdoc />
		bool IAppSettings.CreateErrorForInvalidDate
		{
			get =>
				this.cachedSettings.GetBooleanValue(
					AppSettingsConstants.CreateErrorForInvalidDateKey,
					AppSettingsConstants.CreateErrorForInvalidDateDefaultValue);
			set => this.cachedSettings[AppSettingsConstants.CreateErrorForInvalidDateKey] = value;
		}

		/// <inheritdoc />
		bool IAppSettings.DisableThrowOnIllegalCharacters
		{
			get =>
				this.cachedSettings.GetBooleanValue(
					AppSettingsConstants.DisableThrowOnIllegalCharactersKey,
					AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue);
			set => this.cachedSettings[AppSettingsConstants.DisableThrowOnIllegalCharactersKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.ExportErrorNumberOfRetries
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.ExportErrorNumberOfRetriesKey,
					AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue,
					AppSettingsConstants.ExportErrorNumberOfRetriesMinValue);
			set => this.cachedSettings[AppSettingsConstants.ExportErrorNumberOfRetriesKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.ExportErrorWaitTimeInSeconds
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.ExportErrorWaitTimeInSecondsKey,
					AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue,
					AppSettingsConstants.ExportErrorWaitTimeInSecondsMinValue);
			set => this.cachedSettings[AppSettingsConstants.ExportErrorWaitTimeInSecondsKey] = value;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Globalization",
			"CA1308:NormalizeStringsToUppercase",
			Justification = "This is required for backwards compatibility.")]
		bool IAppSettings.ForceFolderPreview
		{
			get
			{
				string value = GetRegistryKeyValue(AppSettingsConstants.ForceFolderPreviewKey);
				if (string.IsNullOrEmpty(value))
				{
					SetRegistryKeyValue(
						AppSettingsConstants.ForceFolderPreviewKey,
						AppSettingsConstants.ForceFolderPreviewDefaultValue.ToString().ToLowerInvariant());
					return AppSettingsConstants.ForceFolderPreviewDefaultValue;
				}

				return string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0;
			}

			set =>
				SetRegistryKeyValue(
					AppSettingsConstants.ForceFolderPreviewKey,
					value.ToString().ToLowerInvariant());
		}

		/// <inheritdoc />
		int IAppSettings.IoErrorNumberOfRetries
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.IoErrorNumberOfRetriesKey,
					AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue,
					AppSettingsConstants.IoErrorNumberOfRetriesMinValue);
			set => this.cachedSettings[AppSettingsConstants.IoErrorNumberOfRetriesKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.IoErrorWaitTimeInSeconds
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.IoErrorWaitTimeInSecondsKey,
					AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue,
					AppSettingsConstants.IoErrorWaitTimeInSecondsMinValue);
			set => this.cachedSettings[AppSettingsConstants.IoErrorWaitTimeInSecondsKey] = value;
		}

		/// <inheritdoc />
		bool IAppSettings.LogAllEvents
		{
			get =>
				this.cachedSettings.GetBooleanValue(
					AppSettingsConstants.LogAllEventsKey,
					AppSettingsConstants.LogAllEventsDefaultValue);
			set => this.cachedSettings[AppSettingsConstants.LogAllEventsKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.MaximumFilesForTapiBridge
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.MaximumFilesForTapiBridgeKey,
					AppSettingsConstants.MaximumFilesForTapiBridgeDefaultValue,
					AppSettingsConstants.MaximumFilesForTapiBridgeMinValue);
			set => this.cachedSettings[AppSettingsConstants.MaximumFilesForTapiBridgeKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.MaxNumberOfFileExportTasks
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.MaxNumberOfFileExportTasksKey,
					AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue,
					AppSettingsConstants.MaxNumberOfFileExportTasksMinValue);
			set => this.cachedSettings[AppSettingsConstants.MaxNumberOfFileExportTasksKey] = value;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2227:CollectionPropertiesShouldBeReadOnly",
			Justification = "This is required for backwards compatibility.")]
		IList<int> IAppSettings.ObjectFieldIdListContainsArtifactId
		{
			get
			{
				string value = GetRegistryKeyValue(AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey);
				if (string.IsNullOrEmpty(value))
				{
					SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey,
						string.Empty);
					return new List<int>();
				}

				return value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
			}

			set
			{
				SetRegistryKeyValue(
					AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey,
					string.Join(",", value.Select(x => x.ToString())));
			}
		}

		/// <inheritdoc />
		Uri IAppSettings.ProgrammaticWebApiServiceUrl
		{
			get => this.cachedSettings.GetUriValue(AppSettingsConstants.ProgrammaticWebApiServiceUrlKey, null);
			set => this.cachedSettings[AppSettingsConstants.ProgrammaticWebApiServiceUrlKey] = value;
		}

		/// <inheritdoc />
		int IAppSettings.TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get =>
				this.cachedSettings.GetInt32Value(
					AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsKey,
					AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue,
					AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsMinValue);
			set => this.cachedSettings[AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsKey] = value;
		}

		/// <inheritdoc />
		Uri IAppSettings.WebApiServiceUrl
		{
			get
			{
				Uri value = ((IAppSettings)this).ProgrammaticWebApiServiceUrl;
				if (value == null)
				{
					value = this.cachedSettings.GetUriValue(AppSettingsConstants.WebApiServiceUrl, null);
				}

				if (value == null)
				{
					string url = GetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrl);
					if (!string.IsNullOrEmpty(url))
					{
						url = ValidateUriFormat(url);
						if (!string.IsNullOrEmpty(url))
						{
							value = new Uri(url, UriKind.Absolute);
						}
					}
				}

				return value;
			}

			// This is only stored within the Registry.
			set =>
				SetRegistryKeyValue(
					AppSettingsConstants.WebApiServiceUrl,
					value != null ? value.ToString() : string.Empty);
		}

		/// <summary>
		/// Gets the application settings singleton instance.
		/// </summary>
		internal static IAppSettings Instance { get; } = new AppSettings();

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
		/// Clears all settings and retrieve the latest values.
		/// </summary>
		public static void Refresh()
		{
			Instance.Refresh();
		}

		/// <inheritdoc />
		AppSettingsDto IAppSettings.DeepCopy()
		{
			return new AppSettingsDto(this);
		}

		/// <inheritdoc />
		void IAppSettings.Refresh()
		{
			this.cachedSettings.Clear();

			// For backwards compatibility, support all legacy sections.
			this.ReadSectionValues("kCura.Windows.Process");
			this.ReadSectionValues("kCura.Config");
			this.ReadSectionValues("kCura.Utility");
			this.ReadSectionValues("kCura.WinEDDS");
			this.ReadSectionValues("Relativity.Import.Export");
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
		private static string GetRegistryKeyValue(string keyName)
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
		private static string ValidateUriFormat(string value)
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
		private void ReadSectionValues(string sectionName)
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
					this.cachedSettings[key] = sectionDictionary[key];
				}
			}
			catch (ConfigurationErrorsException)
			{
				// Due to legacy concerns, never allow configuration errors to throw.
			}
		}
	}
}
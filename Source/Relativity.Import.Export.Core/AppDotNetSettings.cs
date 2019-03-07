// ----------------------------------------------------------------------------
// <copyright file="AppDotNetSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Represents a class object that provide a thread-safe copy of all .NET application settings.
	/// </summary>
	[Serializable]
	public sealed class AppDotNetSettings : IAppSettings
	{
		/// <summary>
		/// The Web API service backing.
		/// </summary>
		/// <remarks>
		/// This includes a backing field due to the setting complexity.
		/// </remarks>
		private Uri webApiServiceUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppDotNetSettings"/> class.
		/// </summary>
		internal AppDotNetSettings()
		{
			IAppSettings thisSettings = this;
			thisSettings.CreateErrorForInvalidDate = AppSettingsConstants.CreateErrorForInvalidDateDefaultValue;
			thisSettings.DisableThrowOnIllegalCharacters = AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue;
			thisSettings.ExportErrorNumberOfRetries = AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue;
			thisSettings.ExportErrorWaitTimeInSeconds = AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue;
			thisSettings.ForceFolderPreview = AppSettingsConstants.ForceFolderPreviewDefaultValue;
			thisSettings.HttpTimeoutSeconds = AppSettingsConstants.HttpTimeoutSecondsDefaultValue;
			thisSettings.IoErrorNumberOfRetries = AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue;
			thisSettings.IoErrorWaitTimeInSeconds = AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue;
			thisSettings.MaxNumberOfFileExportTasks = AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue;
			thisSettings.MaximumFilesForTapiBridge = AppSettingsConstants.MaximumFilesForTapiBridgeDefaultValue;
			thisSettings.LogAllEvents = AppSettingsConstants.LogAllEventsDefaultValue;
			thisSettings.ObjectFieldIdListContainsArtifactId = null;
			thisSettings.PermissionErrorsRetry = AppSettingsConstants.PermissionErrorsRetryKeyDefaultValue;
			thisSettings.ProgrammaticWebApiServiceUrl = null;
			thisSettings.TapiBridgeExportTransferWaitingTimeInSeconds = AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue;
			thisSettings.TapiPreserveFileTimestamps = AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue;
			this.webApiServiceUrl = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppDotNetSettings"/> class.
		/// </summary>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="settings"/> is <see langword="null" />.
		/// </exception>
		internal AppDotNetSettings(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			IAppSettings thisSettings = this;
			thisSettings.CreateErrorForInvalidDate = settings.CreateErrorForInvalidDate;
			thisSettings.DisableThrowOnIllegalCharacters = settings.DisableThrowOnIllegalCharacters;
			thisSettings.ExportErrorNumberOfRetries = settings.ExportErrorNumberOfRetries;
			thisSettings.ExportErrorWaitTimeInSeconds = settings.ExportErrorWaitTimeInSeconds;
			thisSettings.ForceFolderPreview = settings.ForceFolderPreview;
			thisSettings.HttpTimeoutSeconds = settings.HttpTimeoutSeconds;
			thisSettings.IoErrorNumberOfRetries = settings.IoErrorNumberOfRetries;
			thisSettings.IoErrorWaitTimeInSeconds = settings.IoErrorWaitTimeInSeconds;
			thisSettings.LogAllEvents = settings.LogAllEvents;
			thisSettings.MaxNumberOfFileExportTasks = settings.MaxNumberOfFileExportTasks;
			thisSettings.MaximumFilesForTapiBridge = settings.MaximumFilesForTapiBridge;
			if (settings.ObjectFieldIdListContainsArtifactId != null)
			{
				thisSettings.ObjectFieldIdListContainsArtifactId = new List<int>(settings.ObjectFieldIdListContainsArtifactId);
			}

			thisSettings.PermissionErrorsRetry = settings.PermissionErrorsRetry;
			if (settings.ProgrammaticWebApiServiceUrl != null)
			{
				thisSettings.ProgrammaticWebApiServiceUrl = new Uri(settings.ProgrammaticWebApiServiceUrl.ToString());
			}

			thisSettings.TapiBridgeExportTransferWaitingTimeInSeconds = settings.TapiBridgeExportTransferWaitingTimeInSeconds;
			thisSettings.TapiPreserveFileTimestamps = settings.TapiPreserveFileTimestamps;
			if (settings.WebApiServiceUrl != null)
			{
				this.webApiServiceUrl = new Uri(settings.WebApiServiceUrl.ToString());
			}
		}

		/// <inheritdoc />
		bool IAppSettings.CreateErrorForInvalidDate
		{
			get;
			set;
		}

		/// <inheritdoc />
		bool IAppSettings.DisableThrowOnIllegalCharacters
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.ExportErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.ExportErrorWaitTimeInSeconds
		{
			get;
			set;
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
				string value = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.ForceFolderPreviewKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsReader.SetRegistryKeyValue(
						AppSettingsConstants.ForceFolderPreviewKey,
						AppSettingsConstants.ForceFolderPreviewDefaultValue.ToString().ToLowerInvariant());
					return AppSettingsConstants.ForceFolderPreviewDefaultValue;
				}

				return string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0;
			}

			set =>
				AppSettingsReader.SetRegistryKeyValue(
					AppSettingsConstants.ForceFolderPreviewKey,
					value.ToString().ToLowerInvariant());
		}

		/// <inheritdoc />
		int IAppSettings.HttpTimeoutSeconds
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.IoErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.IoErrorWaitTimeInSeconds
		{
			get;
			set;
		}

		/// <inheritdoc />
		bool IAppSettings.LogAllEvents
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.MaximumFilesForTapiBridge
		{
			get;
			set;
		}

		/// <inheritdoc />
		int IAppSettings.MaxNumberOfFileExportTasks
		{
			get;
			set;
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
				string value = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsReader.SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey,
						string.Empty);
					return new List<int>();
				}

				return value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
			}

			set
			{
				if (value != null)
				{
					AppSettingsReader.SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdKey,
						string.Join(",", value.Select(x => x.ToString())));
				}
			}
		}

		/// <inheritdoc />
		bool IAppSettings.PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		Uri IAppSettings.ProgrammaticWebApiServiceUrl
		{
			get;
			set;
		}

		/// <inheritdoc />
		Relativity.Import.Export.Io.RetryOptions IAppSettings.RetryOptions
		{
			get
			{
				// Always use other settings to drive these enum switches.
				Relativity.Import.Export.Io.RetryOptions value = AppSettingsConstants.RetryOptionsDefaultValue;
				if (((IAppSettings)this).PermissionErrorsRetry)
				{
					value |= Relativity.Import.Export.Io.RetryOptions.Permissions;
				}
				else
				{
					value &= ~Relativity.Import.Export.Io.RetryOptions.Permissions;
				}

				return value;
			}
		}

		/// <inheritdoc />
		int IAppSettings.TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get;
			set;
		}

		/// <inheritdoc />
		bool IAppSettings.TapiPreserveFileTimestamps
		{
			get;
			set;
		}

		/// <inheritdoc />
		Uri IAppSettings.WebApiServiceUrl
		{
			get
			{
				Uri value = ((IAppSettings)this).ProgrammaticWebApiServiceUrl;
				if (value == null)
				{
					value = this.webApiServiceUrl;
				}

				if (value == null)
				{
					string url = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlKey);
					if (!string.IsNullOrEmpty(url))
					{
						url = AppSettingsReader.ValidateUriFormat(url);
						if (!string.IsNullOrEmpty(url))
						{
							value = new Uri(url, UriKind.Absolute);
						}
					}
				}

				return value;
			}

			set =>
				AppSettingsReader.SetRegistryKeyValue(
					AppSettingsConstants.WebApiServiceUrlKey,
					value != null ? value.ToString() : string.Empty);
		}

		/// <inheritdoc />
		IAppSettings IAppSettings.DeepCopy()
		{
			return new AppDotNetSettings(this);
		}

		/// <summary>
		/// Refreshes the web API service URL.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		public void RefreshWebApiServiceUrl(Uri value)
		{
			// This method avoids setting WebApiServiceUrl during a refresh.
			this.webApiServiceUrl = value;
		}
	}
}
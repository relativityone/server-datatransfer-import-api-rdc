// ----------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;

	using Relativity.Import.Export.Io;

	/// <summary>
	/// Defines static properties to obtain application settings.
	/// </summary>
	/// <remarks>
	/// Always favor constructor injecting <see cref="IAppSettings"/>. This is intended for legacy code only.
	/// </remarks>
	public static class AppSettings
	{
		/// <summary>
		/// The thread synchronization object.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The singleton backing field.
		/// </summary>
		private static IAppSettings instance;

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
		/// Gets or sets the HTTP timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public static int HttpTimeoutSeconds
		{
			get => Instance.HttpTimeoutSeconds;
			set => Instance.HttpTimeoutSeconds = value;
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
		/// Gets or sets a value indicating whether permission specific errors are retried.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry permissions specific errors; otherwise, <see langword="false" />.
		/// </value>
		public static bool PermissionErrorsRetry
		{
			get => Instance.PermissionErrorsRetry;
			set => Instance.PermissionErrorsRetry = value;
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
		/// Gets the retry options used by all retry policy blocks. This value is set through a combination of other setting values.
		/// </summary>
		/// <value>
		/// The <see cref="RetryOptions"/> value.
		/// </value>
		public static RetryOptions RetryOptions => Instance.RetryOptions;

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
		/// Gets or sets a value indicating whether to preserve import and export file timestamps.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to preserve file timestamps; otherwise, <see langword="false" />.
		/// </value>
		public static bool TapiPreserveFileTimestamps
		{
			get => Instance.TapiPreserveFileTimestamps;
			set => Instance.TapiPreserveFileTimestamps = value;
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

		/// <summary>
		/// Gets the application settings singleton instance.
		/// </summary>
		internal static IAppSettings Instance
		{
			get
			{
				if (instance == null)
				{
					lock (SyncRoot)
					{
						if (instance == null)
						{
							instance = Create();
						}
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Create a new application settings instance and reads all values from their respective sources.
		/// </summary>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		public static IAppSettings Create()
		{
			return AppSettingsReader.Read();
		}

		/// <summary>
		/// Refreshes the current application settings from their respective sources.
		/// </summary>
		public static void Refresh()
		{
			Refresh(Instance);
		}

		/// <summary>
		/// Refreshes the specified application settings from their respective sources.
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

			AppSettingsReader.Refresh(settings);
		}
	}
}
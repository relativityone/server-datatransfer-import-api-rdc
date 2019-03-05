// ----------------------------------------------------------------------------
// <copyright file="IAppSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents an abstract object that provides thread-safe general import/export application settings.
	/// </summary>
	/// <remarks>
	/// Consider exposing this object to Import API.
	/// </remarks>
	public interface IAppSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether to create an error for an invalid date.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create an error; otherwise, <see langword="false" />.
		/// </value>
		bool CreateErrorForInvalidDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable throwing exceptions when illegal characters are found within a path.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable throwing an exception; otherwise, <see langword="false" />.
		/// </value>
		bool DisableThrowOnIllegalCharacters
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		int ExportErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int ExportErrorWaitTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force a folder preview.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force a folder preview; otherwise, <see langword="false" />.
		/// </value>
		bool ForceFolderPreview
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		int IoErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int IoErrorWaitTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to log all the I/O events.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to log all the I/O events; otherwise, <see langword="false" />.
		/// </value>
		bool LogAllEvents
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of files for each Transfer API bridge instance.
		/// </summary>
		/// <value>
		/// The maximum number of files.
		/// </value>
		int MaximumFilesForTapiBridge
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of file export tasks.
		/// </summary>
		/// <value>
		/// The maximum number of tasks.
		/// </value>
		int MaxNumberOfFileExportTasks
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list of artifacts to use for object field mapping.
		/// </summary>
		/// <value>
		/// The list of artifacts.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2227:CollectionPropertiesShouldBeReadOnly",
			Justification = "This is required for backwards compatibility.")]
		IList<int> ObjectFieldIdListContainsArtifactId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether permission specific errors are retried.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry permissions specific errors; otherwise, <see langword="false" />.
		/// </value>
		bool PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the programmatic Relativity Web API service URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		Uri ProgrammaticWebApiServiceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the retry options used by all retry policy blocks. This value is read-only because the value is driven by a combination of other setting values such as <see cref="PermissionErrorsRetry"/>.
		/// </summary>
		/// <value>
		/// The <see cref="RetryOptions"/> value.
		/// </value>
		Relativity.Import.Export.Io.RetryOptions RetryOptions
		{
			get;
		}

		/// <summary>
		/// Gets or sets the time, in seconds, that a Transfer API bridge waits before releasing the wait handle.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity Web API service URL. This will always return <see cref="ProgrammaticWebApiServiceUrl"/> and then this value. If none are defined, a final check is made with the Windows Registry to determine if it has been set of the RDC.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		Uri WebApiServiceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Performs a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		IAppSettings DeepCopy();
	}
}
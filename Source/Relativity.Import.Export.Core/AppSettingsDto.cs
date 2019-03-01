// ----------------------------------------------------------------------------
// <copyright file="AppSettingsDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a class object that provide a thread-safe copy of all application settings.
	/// </summary>
	[Serializable]
	public sealed class AppSettingsDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingsDto"/> class.
		/// </summary>
		public AppSettingsDto()
		{
			this.CreateErrorForInvalidDate = AppSettingsConstants.CreateErrorForInvalidDateDefaultValue;
			this.DisableThrowOnIllegalCharacters = AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue;
			this.ExportErrorNumberOfRetries = AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue;
			this.ExportErrorWaitTimeInSeconds = AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue;
			this.ForceFolderPreview = AppSettingsConstants.ForceFolderPreviewDefaultValue;
			this.IoErrorNumberOfRetries = AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue;
			this.IoErrorWaitTimeInSeconds = AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue;
			this.LogAllEvents = AppSettingsConstants.LogAllEventsDefaultValue;
			this.ObjectFieldIdListContainsArtifactId = null;
			this.ProgrammaticWebApiServiceUrl = null;
			this.WebApiServiceUrl = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingsDto"/> class.
		/// </summary>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="settings"/> is <see langword="null" />.
		/// </exception>
		public AppSettingsDto(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			this.CreateErrorForInvalidDate = settings.CreateErrorForInvalidDate;
			this.DisableThrowOnIllegalCharacters = settings.DisableThrowOnIllegalCharacters;
			this.ExportErrorNumberOfRetries = settings.ExportErrorNumberOfRetries;
			this.ExportErrorWaitTimeInSeconds = settings.ExportErrorWaitTimeInSeconds;
			this.ForceFolderPreview = settings.ForceFolderPreview;
			this.IoErrorNumberOfRetries = settings.IoErrorNumberOfRetries;
			this.IoErrorWaitTimeInSeconds = settings.IoErrorWaitTimeInSeconds;
			this.LogAllEvents = settings.LogAllEvents;
			if (settings.ObjectFieldIdListContainsArtifactId != null)
			{
				this.ObjectFieldIdListContainsArtifactId = new List<int>(settings.ObjectFieldIdListContainsArtifactId);
			}

			this.ProgrammaticWebApiServiceUrl = settings.ProgrammaticWebApiServiceUrl;
			this.WebApiServiceUrl = settings.WebApiServiceUrl;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to create an error for an invalid date.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create an error; otherwise, <see langword="false" />.
		/// </value>
		public bool CreateErrorForInvalidDate
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
		public bool DisableThrowOnIllegalCharacters
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
		public int ExportErrorNumberOfRetries
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
		public int ExportErrorWaitTimeInSeconds
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
		public bool ForceFolderPreview
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
		public int IoErrorNumberOfRetries
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
		public int IoErrorWaitTimeInSeconds
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
		public bool LogAllEvents
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
		public IList<int> ObjectFieldIdListContainsArtifactId
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
		public Uri ProgrammaticWebApiServiceUrl
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
		public Uri WebApiServiceUrl
		{
			get;
			set;
		}
	}
}
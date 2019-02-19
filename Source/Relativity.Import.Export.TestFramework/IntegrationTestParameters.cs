﻿// ----------------------------------------------------------------------------
// <copyright file="IntegrationTestParameters.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;

	/// <summary>
	/// Represents the parameters used by all integration tests.
	/// </summary>
	public class IntegrationTestParameters
	{
		/// <summary>
		/// The maximum folder length.
		/// </summary>
		public const int MaxFolderLength = 255;

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestParameters"/> class.
		/// </summary>
		public IntegrationTestParameters()
		{
			this.WorkspaceId = 0;
			this.FileShareUncPath = null;
			this.RelativityPassword = null;
			this.RelativityRestUrl = null;
			this.RelativityServicesUrl = null;
			this.RelativityUrl = null;
			this.RelativityUserName = null;
			this.RelativityWebApiUrl = null;
			this.SkipAsperaModeTests = false;
			this.SkipDirectModeTests = false;
			this.SkipIntegrationTests = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestParameters"/> class.
		/// </summary>
		/// <param name="copy">
		/// The object to copy.
		/// </param>
		public IntegrationTestParameters(IntegrationTestParameters copy)
		{
			if (copy == null)
			{
				throw new ArgumentNullException(nameof(copy));
			}

			this.FileShareUncPath = copy.FileShareUncPath;
			this.RelativityPassword = copy.RelativityPassword;
			this.RelativityRestUrl = new Uri(copy.RelativityRestUrl.ToString());
			this.RelativityServicesUrl = new Uri(copy.RelativityServicesUrl.ToString());
			this.RelativityUrl = new Uri(copy.RelativityUrl.ToString());
			this.RelativityUserName = copy.RelativityUserName;
			this.RelativityWebApiUrl = new Uri(copy.RelativityWebApiUrl.ToString());
			this.SkipAsperaModeTests = copy.SkipAsperaModeTests;
			this.SkipDirectModeTests = copy.SkipDirectModeTests;
			this.SkipIntegrationTests = copy.SkipIntegrationTests;
			this.SqlAdminPassword = copy.SqlAdminPassword;
			this.SqlAdminUserName = copy.SqlAdminUserName;
			this.SqlDropWorkspaceDatabase = copy.SqlDropWorkspaceDatabase;
			this.SqlInstanceName = copy.SqlInstanceName;
			this.WorkspaceId = copy.WorkspaceId;
		}

		/// <summary>
		/// Gets or sets the full UNC path to the file share.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string FileShareUncPath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity password used to authenticate.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
#if !DEBUG
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
#endif
		public string RelativityPassword
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity REST API URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri RelativityRestUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity services API URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri RelativityServicesUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri RelativityUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity user name used to authenticate.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		public string RelativityUserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity WebAPI URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri RelativityWebApiUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL admin password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
#if !DEBUG
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
#endif
		public string SqlAdminPassword
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL admin user name.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		public string SqlAdminUserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to drop the test workspace SQL database.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to drop the workspace SQL database; otherwise, <see langword="false" />.
		/// </value>
		public bool SqlDropWorkspaceDatabase
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL instance name.
		/// </summary>
		/// <value>
		/// The SQL instance name.
		/// </value>
		public string SqlInstanceName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip tests that specify the <see cref="Relativity.Transfer.WellKnownTransferClient.Aspera"/> transfer client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the tests; otherwise, <see langword="false" />.
		/// </value>
		public bool SkipAsperaModeTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip tests that specify the <see cref="Relativity.Transfer.WellKnownTransferClient.FileShare"/> transfer client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the tests; otherwise, <see langword="false" />.
		/// </value>
		public bool SkipDirectModeTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip integration tests.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the integration tests; otherwise, <see langword="false" />.
		/// </value>
		public bool SkipIntegrationTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the test workspace artifact identifier.
		/// </summary>
		/// <value>
		/// The artifact identifier.
		/// </value>
		public int WorkspaceId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the workspace template used when creating a test workspace.
		/// </summary>
		/// <value>
		/// The template name.
		/// </value>
		public string WorkspaceTemplate
		{
			get;
			set;
		}

		/// <summary>
		/// Performs a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </returns>
		public IntegrationTestParameters DeepCopy()
		{
			return new IntegrationTestParameters(this);
		}
	}
}
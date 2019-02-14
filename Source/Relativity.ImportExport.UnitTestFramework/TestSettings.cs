// ----------------------------------------------------------------------------
// <copyright file="TestSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.ImportExport.UnitTestFramework
{
	using System;

	/// <summary>
	/// Represents static test settings used throughout the sample.
	/// </summary>
	public static class TestSettings
	{
		/// <summary>
		/// The maximum folder length.
		/// </summary>
		public const int MaxFolderLength = 255;

		/// <summary>
		/// The standard skip test message.
		/// </summary>
		public const string SkipTestMessage = "This test is conditionally skipped - Reason: {0}";

		/// <summary>
		/// Gets or sets the full UNC path to the file share.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public static string FileShareUncPath
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
		public static string RelativityPassword
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
		public static Uri RelativityRestUrl
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
		public static Uri RelativityServicesUrl
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
		public static Uri RelativityUrl
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
		public static string RelativityUserName
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
		public static Uri RelativityWebApiUrl
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
		public static string SqlAdminPassword
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
		public static string SqlAdminUserName
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
		public static bool SqlDropWorkspaceDatabase
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
		public static string SqlInstanceName
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
		public static bool SkipAsperaModeTests
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
		public static bool SkipDirectModeTests
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
		public static bool SkipIntegrationTests
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
		public static int WorkspaceId
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
		public static string WorkspaceTemplate
		{
			get;
			set;
		}
	}
}
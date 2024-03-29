﻿// // ----------------------------------------------------------------------------
// <copyright file="RunningContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using Relativity.DataExchange.Service;

	/// <inheritdoc />
	public class RunningContext : IRunningContext
	{
		private string callingAssembly = string.Empty;

		/// <inheritdoc />
		public Version ImportApiSdkVersion => typeof(IAppSettings).Assembly.GetName().Version;

		/// <inheritdoc />
		public Version ImportExportWebApiVersion { get; set; } = new Version();

		/// <inheritdoc />
		public Version RelativityVersion { get; set; } = new Version();

		/// <inheritdoc />
		public ExecutionSource ExecutionSource { get; set; } = ExecutionSource.Unknown;

		/// <inheritdoc />
		public string ApplicationName { get; set; } = string.Empty;

		/// <inheritdoc />
		public string CallingAssembly
		{
			get => this.callingAssembly;

			set
			{
				if (value != System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
				{
					this.callingAssembly = value;
				}
			}
		}
	}
}
// // ----------------------------------------------------------------------------
// <copyright file="IRunningContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------
namespace Relativity.DataExchange
{
	using System;

	using Relativity.DataExchange.Service;

	/// <summary>
	/// Contains information about the context in which jobs are executed.
	/// </summary>
	public interface IRunningContext
	{
		/// <summary>
		/// Gets version of import API SDK.
		/// </summary>
		Version ImportApiSdkVersion { get; }

		/// <summary>
		/// Gets or sets version of import export web API.
		/// </summary>
		Version ImportExportWebApiVersion { get; set; }

		/// <summary>
		/// Gets or sets version of Relativity.
		/// </summary>
		Version RelativityVersion { get; set; }

		/// <summary>
		/// Gets or sets job's execution source.
		/// </summary>
		ExecutionSource ExecutionSource { get; set; }

		/// <summary>
		/// Gets or sets name of application executing job. This property is more customizable alternative to <see cref="ExecutionSource"/>.
		/// </summary>
		string ApplicationName { get; set; }
	}
}
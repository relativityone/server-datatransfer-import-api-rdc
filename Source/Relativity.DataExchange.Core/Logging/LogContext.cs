// <copyright file="LogContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Logging
{
	using kCura.Vendor.Castle.Core.Internal;

	/// <summary>
	/// Log context for import job.
	/// </summary>
	internal class LogContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogContext"/> class.
		/// </summary>
		/// <param name="runId">job run id.</param>
		/// <param name="workspaceId">workspace id.</param>
		public LogContext(string runId, int workspaceId)
		{
			this.RunId = runId;
			this.WorkspaceId = workspaceId;
		}

		/// <summary>
		/// Gets unique job identifier.
		/// </summary>
		public string RunId { get; }

		/// <summary>
		/// Gets job Workspace Id.
		/// </summary>
		public int WorkspaceId { get; }
	}
}

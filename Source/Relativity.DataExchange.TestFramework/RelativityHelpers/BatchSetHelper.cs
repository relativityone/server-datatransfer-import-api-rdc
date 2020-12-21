// ----------------------------------------------------------------------------
// <copyright file="BatchSetHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Threading.Tasks;

	using Relativity.Services.Batching;
	using Relativity.Services.Review.Batching;
	using Relativity.Services.Review.Batching.Models;

	public static class BatchSetHelper
	{
		public static async Task<BatchSet> CreateBatchSetAsync(
			IntegrationTestParameters parameters,
			DataSource dataSource,
			int workspaceId,
			string batchSetName,
			string batchSetPrefix,
			int batchSetSize)
		{
			using (IBatchingManager batchingManager = ServiceHelper.GetServiceProxy<IBatchingManager>(parameters))
			{
				var batchSet = new BatchSet
				{
					Name = batchSetName,
					BatchSize = batchSetSize,
					BatchPrefix = batchSetPrefix,
					DataSource = dataSource,
				};

				return await batchingManager.CreateBatchSetAsync(workspaceId, batchSet).ConfigureAwait(false);
			}
		}

		public static async Task<BatchProcessResult> CreateBatchesAsync(IntegrationTestParameters parameters, BatchSet batchSet, int workspaceId)
		{
			using (IBatchingManager batchingManager = ServiceHelper.GetServiceProxy<IBatchingManager>(parameters))
			{
				return await batchingManager.CreateBatchesAsync(workspaceId, batchSet.ArtifactID).ConfigureAwait(false);
			}
		}
	}
}

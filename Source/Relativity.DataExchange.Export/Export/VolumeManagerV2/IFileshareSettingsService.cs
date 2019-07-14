namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IFileShareSettingsService
	{
		/// <summary>
		/// Asynchronously reads all available file shares associated with the workspace and returns
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns>
		/// The <see cref="Task"/> instance.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when an exception occurs retrieving the file shares.
		/// </exception>
		Task ReadFileSharesAsync(CancellationToken token);

		/// <summary>
		/// Gets the settings for file share.
		/// </summary>
		/// <param name="artifactId">
		/// The artifact unique identifier.
		/// </param>
		/// <param name="path">
		/// The remote or server path.
		/// </param>
		/// <returns>
		/// The <see cref="IRelativityFileShareSettings"/> instances.
		/// </returns>
		IRelativityFileShareSettings GetSettingsForFileShare(int artifactId, string path);
	}
}
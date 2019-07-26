namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using Relativity.Transfer;

	/// <summary>
	/// Represents an abstract file share object.
	/// </summary>
	public interface IRelativityFileShareSettings
	{
		/// <summary>
		/// Gets the artifact identifier for the resource server.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int ArtifactId
		{
			get;
		}

		/// <summary>
		/// Gets the optional transfer credential for this file share.
		/// </summary>
		/// <value>
		/// The <see cref="Credential"/> instance.
		/// </value>
		Credential TransferCredential
		{
			get;
		}

		/// <summary>
		/// Gets the UNC path.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		string UncPath
		{
			get;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">
		/// The <see cref="System.Object" /> to compare with this instance.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" /> to default all values.
		/// </returns>
		bool Equals(object obj);

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		int GetHashCode();

		/// <summary>
		/// Determines whether the current file share is the base of the specified path.
		/// </summary>
		/// <param name="path">The path to test.</param>
		/// <returns>
		/// <see langword="true" /> if the current file share is the base of the specified path; otherwise, <see langword="false" /> to default all values.
		/// </returns>
		bool IsBaseOf(string path);
	}
}
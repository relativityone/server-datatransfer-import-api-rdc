namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using System;

	using Relativity.Transfer;

	/// <summary>
	/// Represents a file share class object.
	/// Implements the <see cref="Relativity.DataExchange.Export.VolumeManagerV2.IRelativityFileShareSettings" />
	/// </summary>
	/// <seealso cref="Relativity.DataExchange.Export.VolumeManagerV2.IRelativityFileShareSettings" />
	public class RelativityFileShareSettings : IRelativityFileShareSettings
	{
		private readonly RelativityFileShare _fileShare;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityFileShareSettings"/> class.
		/// </summary>
		/// <param name="fileShare">
		/// The file share.
		/// </param>
		public RelativityFileShareSettings(RelativityFileShare fileShare)
		{
			_fileShare = fileShare.ThrowIfNull(nameof(fileShare));
		}

		/// <inheritdoc />
		public int ArtifactId => this._fileShare.ArtifactId;

		/// <inheritdoc />
		public Credential TransferCredential =>
			_fileShare.TransferCredential == null ? null : _fileShare.TransferCredential.CreateCredential();

		/// <inheritdoc />
		public string UncPath => _fileShare.Url;

		/// <inheritdoc />
		public bool IsBaseOf(string path)
		{
			bool result = _fileShare.IsBaseOf(path);
			return result;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			
			RelativityFileShareSettings other = obj as RelativityFileShareSettings;
			if (other == null)
			{
				return false;
			}

			// Note: intentionally excluding the credential.
			bool result = this.ArtifactId == other.ArtifactId && string.Compare(
				              this.UncPath.TrimTrailingSlashFromUrl(),
				              other.UncPath.TrimTrailingSlashFromUrl(),
				              StringComparison.OrdinalIgnoreCase) == 0;
			return result;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				// Note: intentionally excluding the credential.
				const int HashConstant = 397;
				int hashCode = !string.IsNullOrEmpty(this.UncPath) ? this.UncPath.TrimTrailingSlashFromUrl().GetHashCode() : 0;
				hashCode = (hashCode * HashConstant) ^ (this.ArtifactId.GetHashCode());
				return hashCode;
			}
		}
	}
}
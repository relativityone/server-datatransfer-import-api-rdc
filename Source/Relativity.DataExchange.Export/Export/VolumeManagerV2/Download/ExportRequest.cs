namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Globalization;

	using Relativity.DataExchange.Resources;
	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public abstract class ExportRequest
	{
		public string SourceLocation { get; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int ArtifactId { get; }

		public string DestinationLocation { get; }

		public string FileName { get; set; }

		public int Order { get; set; }

		protected ExportRequest(int artifactId, string sourceLocation, string destinationLocation)
		{
			ArtifactId = artifactId;
			SourceLocation = sourceLocation;
			DestinationLocation = destinationLocation;
		}

		public TransferPath CreateTransferPath(int order)
		{
			Order = order;
			return CreateTransferPath();
		}

		protected abstract TransferPath CreateTransferPath();

		protected static TransferPath CreateTransferPath(
			int artifactId,
			int order,
			string sourcePath,
			string targetPath,
			HttpTransferPathData data)
		{
			// Note: the sourcePath can be null.
			if (string.IsNullOrWhiteSpace(targetPath))
			{
				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					ExportStrings.ExportRequestTargetPathExceptionMessage,
					artifactId);
				throw new ArgumentException(errorMessage, nameof(targetPath));
			}

			var fileInfo = new System.IO.FileInfo(targetPath);
			var transferPath = new TransferPath
				                   {
					                   Order = order,
					                   SourcePath = sourcePath,
					                   TargetPath = fileInfo.Directory?.FullName,
					                   TargetFileName = fileInfo.Name
				                   };
			transferPath.AddData(HttpTransferPathData.HttpTransferPathDataKey, data);
			return transferPath;
		}
	}
}
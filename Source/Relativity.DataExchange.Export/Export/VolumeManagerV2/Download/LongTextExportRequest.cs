namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;

	using kCura.WinEDDS.Exporters;

	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	public class LongTextExportRequest : ExportRequest
	{
		private LongTextExportRequest(ObjectExportInfo artifact, string destinationLocation)
			: base(artifact.ArtifactID, Guid.NewGuid().ToString(), destinationLocation)
		{
		}

		/// <summary>
		///     For Web mode
		/// </summary>
		public bool FullText { get; private set; }

		/// <summary>
		///     For Web mode
		/// </summary>
		public int FieldArtifactId { get; private set; }

		public static LongTextExportRequest CreateRequestForFullText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new LongTextExportRequest(artifact, destinationLocation)
			{
				FullText = true,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}

		public static LongTextExportRequest CreateRequestForLongText(ObjectExportInfo artifact, int fieldArtifactId, string destinationLocation)
		{
			var request = new LongTextExportRequest(artifact, destinationLocation)
			{
				FullText = false,
				FieldArtifactId = fieldArtifactId
			};
			return request;
		}

		protected override TransferPath CreateTransferPath()
		{
			HttpTransferPathData httpTransferPathData = new HttpTransferPathData
				                                            {
					                                            ArtifactId = this.ArtifactId,
					                                            ExportType =
						                                            this.FullText
							                                            ? ExportType.FullText
							                                            : ExportType.LongTextFieldArtifact,
					                                            LongTextFieldArtifactId = this.FieldArtifactId
				                                            };

			// Note: The Guid is supplied to avoid TAPI/export argument checks.
			TransferPath transferPath = CreateTransferPath(
				this.ArtifactId,
				this.Order,
				Guid.NewGuid().ToString(),
				this.DestinationLocation,
				httpTransferPathData);
			return transferPath;
		}
	}
}
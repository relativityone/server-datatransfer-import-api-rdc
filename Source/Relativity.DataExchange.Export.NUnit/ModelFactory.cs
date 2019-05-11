// -----------------------------------------------------------------------------------------------------
// <copyright file="ModelFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.Text;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2;
    using Relativity.DataExchange.Export.VolumeManagerV2.Download;
    using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
    using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

    public class ModelFactory
	{
		private static int _artifactId = 1;
		private static int _order = 1;

		public static Native GetNative(NativeRepository nativeRepository)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = _artifactId++,
				NativeSourceLocation = "location"
			};
			ExportRequest exportRequest = new PhysicalFileExportRequest(artifact, "location")
			{
				FileName = Guid.NewGuid().ToString(),
				Order = _order++
			};
			Native native = new Native(artifact)
			{
				HasBeenDownloaded = false,
				ExportRequest = exportRequest
			};
			nativeRepository.Add(native);
			return native;
		}

		public static Image GetImage(int artifactId, ImageRepository imageRepository)
		{
			ImageExportInfo artifact = new ImageExportInfo
			{
				ArtifactID = artifactId,
				SourceLocation = "sourceLocation"
			};
			ExportRequest exportRequest = new PhysicalFileExportRequest(artifact, "location")
			{
				FileName = Guid.NewGuid().ToString(),
				Order = _order++
			};
			Image image = new Image(artifact)
			{
				HasBeenDownloaded = false,
				ExportRequest = exportRequest
			};
			imageRepository.Add(image.InList());
			return image;
		}

		public static LongText GetLongText(int artifactId, LongTextRepository longTextRepository)
		{
			return GetLongTextWithLocationAndEncoding(artifactId, longTextRepository, "location", Encoding.Unicode);
		}

		public static LongText GetLongTextWithLocationAndEncoding(int artifactId, LongTextRepository longTextRepository, string location, Encoding encoding)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId
			};
			LongTextExportRequest exportRequest = LongTextExportRequest.CreateRequestForLongText(artifact, 1, location);
			exportRequest.FileName = Guid.NewGuid().ToString();
			exportRequest.Order = _order++;
			LongText longText = LongText.CreateFromMissingValue(artifactId, 1, exportRequest, encoding);
			longTextRepository.Add(longText.InList());
			return longText;
		}
	}
}
using System;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
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
			FileExportRequest exportRequest = new NativeFileExportRequest(artifact, "location")
			{
				FileName = Guid.NewGuid().ToString(),
				Order = _order++
			};
			Native native = new Native(artifact)
			{
				HasBeenDownloaded = false,
				ExportRequest = exportRequest
			};
			nativeRepository.Add(native.InList());
			return native;
		}

		public static Image GetImage(int artifactId, ImageRepository imageRepository)
		{
			ImageExportInfo artifact = new ImageExportInfo
			{
				ArtifactID = artifactId,
				SourceLocation = "sourceLocation"
			};
			FileExportRequest exportRequest = new NativeFileExportRequest(artifact, "location")
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
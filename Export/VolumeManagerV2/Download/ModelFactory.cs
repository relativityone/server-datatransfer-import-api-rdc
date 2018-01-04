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

		public static Native GetNative(NativeRepository nativeRepository)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = _artifactId++
			};
			ExportRequest exportRequest = new NativeFileExportRequest(artifact, "location")
			{
				UniqueId = Guid.NewGuid().ToString()
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
				ArtifactID = artifactId
			};
			ExportRequest exportRequest = new NativeFileExportRequest(artifact, "location")
			{
				UniqueId = Guid.NewGuid().ToString()
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
			exportRequest.UniqueId = Guid.NewGuid().ToString();
			LongText longText = LongText.CreateFromMissingValue(artifactId, 1, exportRequest, encoding);
			longTextRepository.Add(longText.InList());
			return longText;
		}
	}
}
// -----------------------------------------------------------------------------------------------------
// <copyright file="ModelFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
    using System.IO;
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
			return GetNative(nativeRepository, "sourceLocation", Path.Combine(@"C:\temp", Guid.NewGuid().ToString()));
		}

		public static Native GetNative(NativeRepository nativeRepository, string sourceLocation, string targetFile)
		{
			return GetNative(nativeRepository, sourceLocation, targetFile, _artifactId++);
		}

		public static Native GetNative(NativeRepository nativeRepository, string sourceLocation, string targetFile, int artifactId)
		{
			if (nativeRepository == null)
			{
				throw new ArgumentNullException(nameof(nativeRepository));
			}

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId,
				NativeSourceLocation = sourceLocation
			};

			ExportRequest exportRequest = new PhysicalFileExportRequest(artifact, targetFile)
			{
				FileName = System.IO.Path.GetFileName(targetFile),
				Order = _order++
			};

			Native native = new Native(artifact)
			{
				TransferCompleted = false,
				ExportRequest = exportRequest
			};

			nativeRepository.Add(native);
			return native;
		}

		public static Image GetImage(ImageRepository imageRepository, int artifactId)
		{
			return GetImage(imageRepository, artifactId, "sourceLocation", Guid.NewGuid().ToString());
		}

		public static Image GetImage(ImageRepository imageRepository, int artifactId, string sourceLocation)
		{
			return GetImage(
				imageRepository,
				artifactId,
				sourceLocation,
				Path.Combine(@"C:\temp", Path.Combine(@"C:\temp", Guid.NewGuid().ToString())));
		}

		public static Image GetImage(ImageRepository imageRepository, int artifactId, string sourceLocation, string targetFile)
		{
			if (imageRepository == null)
			{
				throw new ArgumentNullException(nameof(imageRepository));
			}

			ImageExportInfo artifact = new ImageExportInfo
			{
				ArtifactID = artifactId,
				SourceLocation = sourceLocation
			};

			ExportRequest exportRequest = new PhysicalFileExportRequest(artifact, targetFile)
			{
				FileName = System.IO.Path.GetFileName(targetFile),
				Order = _order++
			};

			Image image = new Image(artifact)
			{
				TransferCompleted = false,
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
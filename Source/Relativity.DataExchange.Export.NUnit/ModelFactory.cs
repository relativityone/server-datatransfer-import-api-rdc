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

		public static FileRequest<ObjectExportInfo> GetNative(FileRequestRepository nativeRepository)
		{
			return GetNative(nativeRepository, "sourceLocation", Path.Combine(@"C:\temp", Guid.NewGuid().ToString()));
		}

		public static FileRequest<ObjectExportInfo> GetNative(FileRequestRepository nativeRepository, string sourceLocation, string targetFile)
		{
			return GetNative(nativeRepository, sourceLocation, targetFile, _artifactId++);
		}

		public static FileRequest<ObjectExportInfo> GetNative(FileRequestRepository nativeRepository, string sourceLocation, string targetFile, int artifactId)
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

			ExportRequest exportRequest = PhysicalFileExportRequest.CreateRequestForNative(artifact, targetFile);
			exportRequest.FileName = System.IO.Path.GetFileName(targetFile);
			exportRequest.Order = _order++;

			FileRequest<ObjectExportInfo> native = new FileRequest<ObjectExportInfo>(artifact)
			{
			   TransferCompleted = false,
			   ExportRequest = exportRequest
			};

			nativeRepository.Add(native);
			return native;
		}

		public static FileRequest<ImageExportInfo> GetImage(ImageRepository imageRepository, int artifactId)
		{
			return GetImage(imageRepository, artifactId, "sourceLocation", Guid.NewGuid().ToString());
		}

		public static FileRequest<ImageExportInfo> GetImage(ImageRepository imageRepository, int artifactId, string sourceLocation)
		{
			return GetImage(
				imageRepository,
				artifactId,
				sourceLocation,
				Path.Combine(@"C:\temp", Path.Combine(@"C:\temp", Guid.NewGuid().ToString())));
		}

		public static FileRequest<ImageExportInfo> GetImage(ImageRepository imageRepository, int artifactId, string sourceLocation, string targetFile)
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

			ExportRequest exportRequest = PhysicalFileExportRequest.CreateRequestForImage(artifact, targetFile);
			exportRequest.FileName = System.IO.Path.GetFileName(targetFile);
			exportRequest.Order = _order++;

			FileRequest<ImageExportInfo> image = new FileRequest<ImageExportInfo>(artifact)
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
			LongText longText = LongText.CreateFromMissingValue(artifactId, 1, exportRequest, encoding, artifact.LongTextLength);
			longTextRepository.Add(longText.InList());
			return longText;
		}

		public static FileRequest<ObjectExportInfo> GetPdf(FileRequestRepository pdfRepository, int artifactId)
		{
			return GetPdf(pdfRepository, artifactId, "sourceLocation", Guid.NewGuid().ToString());
		}

		public static FileRequest<ObjectExportInfo> GetPdf(FileRequestRepository pdfRepository, int artifactId, string sourceLocation, string targetFile)
		{
			if (pdfRepository == null)
			{
				throw new ArgumentNullException(nameof(pdfRepository));
			}

			ObjectExportInfo artifact = new ObjectExportInfo()
			{
				ArtifactID = artifactId,
				PdfSourceLocation = sourceLocation
			};

			ExportRequest exportRequest = PhysicalFileExportRequest.CreateRequestForPdf(artifact, targetFile);
			exportRequest.FileName = System.IO.Path.GetFileName(targetFile);
			exportRequest.Order = _order++;

			FileRequest<ObjectExportInfo> pdf = new FileRequest<ObjectExportInfo>(artifact)
			{
				TransferCompleted = false,
				ExportRequest = exportRequest
			};

			pdfRepository.Add(pdf);
			return pdf;
		}
	}
}
// ----------------------------------------------------------------------------
// <copyright file="ImageConversionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Media
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents a class object service to convert images to their multi-page representation.
	/// </summary>
	internal class ImageConversionService : IImageConversionService
	{
		/// <summary>
		/// The file system wrapper.
		/// </summary>
		private readonly IFileSystem fileSystem;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionService"/> class.
		/// </summary>
		public ImageConversionService()
			: this(FileSystem.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionService"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		public ImageConversionService(IFileSystem fileSystem)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			this.fileSystem = fileSystem;
		}

		/// <inheritdoc />
		public void ConvertTiffsToMultiPageTiff(IEnumerable<string> inputFiles, string outputFile)
		{
			if (inputFiles == null)
			{
				throw new ArgumentNullException(nameof(inputFiles));
			}

			List<string> inputFilesList = inputFiles.ToList();
			if (inputFilesList.Count == 0)
			{
				throw new ArgumentOutOfRangeException(
					nameof(inputFiles),
					"The multi-page TIFF conversion requires at least 1 image.");
			}

			if (string.IsNullOrEmpty(outputFile))
			{
				throw new ArgumentNullException(nameof(outputFile));
			}

			System.Drawing.Imaging.ImageCodecInfo info = null;
			foreach (System.Drawing.Imaging.ImageCodecInfo imageCodecInfo in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
			{
				if (string.Compare(imageCodecInfo.MimeType, "image/tiff", StringComparison.OrdinalIgnoreCase) == 0)
				{
					info = imageCodecInfo;
				}
			}

			if (info == null)
			{
				throw new ImageRollupException(Strings.ImageConversionTiffCodecNotFoundMessage);
			}

			outputFile = this.fileSystem.Path.GetFullPath(outputFile);
			System.Drawing.Imaging.Encoder saveFlag = System.Drawing.Imaging.Encoder.SaveFlag;
			System.Drawing.Imaging.Encoder compression = System.Drawing.Imaging.Encoder.Compression;
			System.Drawing.Image multiPageTiff = null;
			int pageNumber = -1;
			System.Drawing.Imaging.EncoderParameters encoderParams = null;

			try
			{
				// Create encoding parameters
				encoderParams = new System.Drawing.Imaging.EncoderParameters(2);
				encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(
					saveFlag,
					(long)System.Drawing.Imaging.EncoderValue.MultiFrame);
				for (pageNumber = 0; pageNumber < inputFilesList.Count; pageNumber++)
				{
					string sourceImageFile = inputFilesList[pageNumber];
					if (pageNumber == 0)
					{
						multiPageTiff = this.GetImage(sourceImageFile);
						encoderParams.Param[1] =
							multiPageTiff.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed
								? new System.Drawing.Imaging.EncoderParameter(
									compression,
									(long)System.Drawing.Imaging.EncoderValue.CompressionCCITT4)
								: new System.Drawing.Imaging.EncoderParameter(
									compression,
									(long)System.Drawing.Imaging.EncoderValue.CompressionLZW);
						multiPageTiff.Save(outputFile, info, encoderParams);
					}
					else
					{
						encoderParams.Dispose();
						encoderParams = null;
						encoderParams = new System.Drawing.Imaging.EncoderParameters(2);
						encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(
							saveFlag,
							(long)System.Drawing.Imaging.EncoderValue.FrameDimensionPage);
						System.Drawing.Image bitmap = this.GetImage(sourceImageFile);
						encoderParams.Param[1] =
							bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed
								? new System.Drawing.Imaging.EncoderParameter(
									compression,
									(long)System.Drawing.Imaging.EncoderValue.CompressionCCITT4)
								: new System.Drawing.Imaging.EncoderParameter(
									compression,
									(long)System.Drawing.Imaging.EncoderValue.CompressionLZW);
						multiPageTiff.SaveAdd(bitmap, encoderParams);
						bitmap.Dispose();
					}

					if (pageNumber == inputFilesList.Count - 1)
					{
						encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(
							saveFlag,
							(long)System.Drawing.Imaging.EncoderValue.Flush);
						multiPageTiff.SaveAdd(encoderParams);
					}
				}

				multiPageTiff?.Dispose();
			}
			catch (Exception e)
			{
				multiPageTiff?.Dispose();
				if (pageNumber > -1)
				{
					throw new ConvertToMultiPageTiffException(
						inputFilesList[pageNumber],
						pageNumber,
						inputFilesList.Count,
						e);
				}

				throw;
			}
			finally
			{
				encoderParams?.Dispose();
			}
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "For backwards compatibility.")]
		public void ConvertImagesToMultiPagePdf(IEnumerable<string> inputFiles, string outputFile)
		{
			if (inputFiles == null)
			{
				throw new ArgumentNullException(nameof(inputFiles));
			}

			List<string> inputFilesList = inputFiles.ToList();
			if (inputFilesList.Count == 0)
			{
				throw new ArgumentOutOfRangeException(
					nameof(inputFiles),
					"The multi-page PDF conversion requires at least 1 image.");
			}

			if (string.IsNullOrEmpty(outputFile))
			{
				throw new ArgumentNullException(nameof(outputFile));
			}

			iTextSharp.text.Document document = null;
			int pageNumber = -1;

			try
			{
				outputFile = this.fileSystem.Path.GetFullPath(outputFile);
				document = new iTextSharp.text.Document();
				using (System.IO.FileStream fs = new System.IO.FileStream(outputFile, System.IO.FileMode.Create))
				{
					var pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);
					document.Open();
					for (pageNumber = 0; pageNumber < inputFilesList.Count; pageNumber++)
					{
						string sourceImageFile = inputFilesList[pageNumber];
						System.Drawing.Image bitmap = this.GetImage(sourceImageFile);
						iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(
							(float)(bitmap.Width / bitmap.HorizontalResolution * 72.0),
							(float)(bitmap.Height / bitmap.VerticalResolution * 72.0));
						document.SetPageSize(pageSize);
						document.NewPage();
						int totalFrames = bitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
						iTextSharp.text.pdf.PdfContentByte contentByte = pdfWriter.DirectContent;
						if (totalFrames > 1)
						{
							for (int i = 0; i < totalFrames; i++)
							{
								bitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);
								iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(
									bitmap,
									System.Drawing.Imaging.ImageFormat.Bmp);
								image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
								float xAlign = (document.PageSize.Width - image.ScaledWidth) / 2;
								float yAlign = (document.PageSize.Height - image.ScaledHeight) / 2;
								image.SetAbsolutePosition(xAlign, yAlign);
								contentByte.AddImage(image);
							}

							bitmap.Dispose();
						}
						else
						{
							bitmap.Dispose();
							using (System.IO.FileStream inputStream = new System.IO.FileStream(
								sourceImageFile,
								System.IO.FileMode.Open))
							{
								iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputStream);
								image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
								float xAlign = (document.PageSize.Width - image.ScaledWidth) / 2;
								float yAlign = (document.PageSize.Height - image.ScaledHeight) / 2;
								image.SetAbsolutePosition(xAlign, yAlign);
								contentByte.AddImage(image);
							}
						}
					}

					document.Close();
				}
			}
			catch (Exception e)
			{
				if (document != null)
				{
					try
					{
						document.Close();
					}
					catch
					{
						// For backwards compatibility.
					}
				}

				if (pageNumber > -1)
				{
					throw new ConvertToMultiPagePdfException(
						inputFilesList[pageNumber],
						pageNumber,
						inputFilesList.Count,
						e);
				}

				throw;
			}
		}

		/// <inheritdoc />
		public int GetTiffImageCount(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			using (System.Drawing.Image image = this.GetImage(file))
			{
				int count = image.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
				return count;
			}
		}

		/// <inheritdoc />
		public int GetPdfPageCount(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			file = this.fileSystem.Path.GetFullPath(file);
			iTextSharp.text.pdf.PdfReader pdfReader = null;
			try
			{
				pdfReader = new iTextSharp.text.pdf.PdfReader(file);
				int count = pdfReader.NumberOfPages;
				return count;
			}
			finally
			{
				pdfReader?.Close();
			}
		}

		/// <summary>
		/// Gets the image for the specified file.
		/// </summary>
		/// <param name="path">
		/// The full path to the image file.
		/// </param>
		/// <returns>
		/// The <see cref="System.Drawing.Image"/> instance.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "For backwards compatibility.")]
		private System.Drawing.Image GetImage(string path)
		{
			System.Drawing.Image image = null;

			try
			{
				path = this.fileSystem.Path.GetFullPath(path);
				image = System.Drawing.Image.FromFile(path);
				return image;
			}
			catch (Exception)
			{
				image?.Dispose();
				image = System.Drawing.Image.FromFile(path);
				return image;
			}
		}
	}
}
// ----------------------------------------------------------------------------
// <copyright file="FreeImageIdService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Media
{
	using System;
	using System.Globalization;
	using System.IO;

	using FreeImageAPI;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents a service class object to identify an image file using the FreeImage native library.
	/// </summary>
	internal class FreeImageIdService : IImageIdService
	{
		/// <inheritdoc />
		public ImageFormat Identify(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
			if (!fileInfo.Exists)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageFileNotFoundError, file);
				throw new FileNotFoundException(message, file);
			}

			FreeImageAPI.FREE_IMAGE_FORMAT imageFormat = FreeImageAPI.FreeImage.GetFileType(file, 0);
			return GetImageFormat(imageFormat);
		}

		/// <inheritdoc />
		public void Validate(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			FreeImageAPI.FIBITMAP image = FIBITMAP.Zero;
			ImageFormat imageFormat = this.Identify(file);
			if (imageFormat == ImageFormat.Tiff)
			{
				image = FreeImageAPI.FreeImage.Load(FreeImageAPI.FREE_IMAGE_FORMAT.FIF_TIFF, file, 0);
			}

			try
			{
				// Determine if the file is a zero-byte image.
				System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
				if (fileInfo.Length == 0)
				{
					string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageZeroBytesError, file);
					throw new ImageValidationException(message);
				}

				// Determine if the file is a supported image type
				switch (imageFormat)
				{
					case ImageFormat.Tiff:

						try
						{
							int bitCount = Convert.ToInt32(FreeImageAPI.FreeImage.GetInfoHeaderEx(image).biBitCount);
							if (bitCount > 1)
							{
								string message = string.Format(
									CultureInfo.CurrentCulture,
									Strings.TiffImageNotOneBitError,
									file,
									bitCount);
								throw new ImageValidationException(message);
							}
						}
						catch (ImageValidationException)
						{
							throw;
						}
						catch (NullReferenceException e)
						{
							string message = string.Format(
								CultureInfo.CurrentCulture,
								Strings.TiffImageNotSupportedError,
								file);
							throw new ImageValidationException(message, e);
						}
						catch (Exception e)
						{
							string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageReadError, file);
							throw new ImageValidationException(message, e);
						}

						const string SupportedEncoding = ImageConstants.TiffEncoding4Fax;
						string encoding = GetTiffEncoding(file);
						if (string.Compare(encoding, SupportedEncoding, StringComparison.OrdinalIgnoreCase) != 0)
						{
							string message = string.Format(
								CultureInfo.CurrentCulture,
								Strings.TiffEncodingNotSupportedError,
								file,
								encoding,
								SupportedEncoding);
							throw new ImageValidationException(message);
						}

						break;

					case ImageFormat.Jpeg:
						// This is OK.
						break;

					default:
						string fileTypeString = imageFormat.ToString();
						if (Microsoft.VisualBasic.CompilerServices.LikeOperator.LikeString(
							fileTypeString,
							"FIF_?*",
							Microsoft.VisualBasic.CompareMethod.Binary))
						{
							fileTypeString = fileTypeString.Substring(4);
						}

						string formatMessage = string.Format(
							CultureInfo.CurrentCulture,
							Strings.ImageFormatNotSupportedError,
							file,
							fileTypeString);
						throw new ImageValidationException(formatMessage);
				}
			}
			finally
			{
				if (image != FIBITMAP.Zero)
				{
					FreeImageAPI.FreeImage.Unload(image);
				}
			}
		}

		/// <summary>
		/// Gets the tiff encoding for the image file.
		/// </summary>
		/// <param name="file">
		/// The full path to the image file.
		/// </param>
		/// <returns>
		/// The encoding.
		/// </returns>
		private static string GetTiffEncoding(string file)
		{
			byte[] offset = new byte[12];
			string returnValue = "Unknown";
			using (System.IO.FileStream fileStream = new System.IO.FileStream(
				file,
				System.IO.FileMode.Open,
				System.IO.FileAccess.Read))
			{
				// Read the Image File Header to determine where the Image File Directory record is located.
				fileStream.Read(offset, 0, 8);
				ByteOrdering byteOrdering = offset[0] == 73 ? ByteOrdering.LittleEndian : ByteOrdering.BigEndian;
				fileStream.Position = ConvertByteArrayToInt64(offset, 4, 4, byteOrdering);

				// Determine the number of Image File Directory entries
				fileStream.Read(offset, 0, 2);
				long imageFileDirectoryEntries = ConvertByteArrayToInt64(offset, 0, 2, byteOrdering);

				// Check to see if this is a multi - page TIFF.
				long currentPosition = fileStream.Position;
				fileStream.Position += imageFileDirectoryEntries * 12;
				fileStream.Read(offset, 0, 4);

				if (ConvertByteArrayToInt64(offset, 0, 4, byteOrdering) > 0)
				{
					string message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TiffMultiPageNotSupportedError,
						file);
					throw new ImageValidationException(message);
				}

				fileStream.Position = currentPosition;

				// Determine the compression type used by looping through the Image File Directory entries
				for (int i = 0; i < imageFileDirectoryEntries; i++)
				{
					fileStream.Read(offset, 0, 12);
					var tagId = ConvertByteArrayToInt64(offset, 0, 2, byteOrdering);
					if (tagId == 259)
					{
						// The type determines whether the value is stored in 2 bytes or 4 bytes. This makes a HUGE difference for Motorola byte ordering.
						// http://partners.adobe.com/public/developer/en/tiff/TIFF6.pdf
						// 3=SHORT (16 bit), 4=LONG(32 bit)
						int valueSize = 2;
						var type = ConvertByteArrayToInt64(offset, 2, 2, byteOrdering);
						if (type == 4)
						{
							valueSize = 4;
						}

						long value = ConvertByteArrayToInt64(offset, 8, valueSize, byteOrdering);
						switch (value)
						{
							case 1:
								returnValue = "None";
								break;

							case 2:
								returnValue = ImageConstants.TiffEncoding3;
								break;

							case 3:
								returnValue = ImageConstants.TiffEncoding3Fax;
								break;

							case 4:
								returnValue = ImageConstants.TiffEncoding4Fax;
								break;

							case 5:
								returnValue = ImageConstants.TiffEncodingLzw;
								break;

							case 6:
								returnValue = ImageConstants.TiffEncodingJpegOld;
								break;

							case 7:
								returnValue = ImageConstants.TiffEncodingJpegNew;
								break;

							case 8:
								returnValue = ImageConstants.TiffEncodingZlib;
								break;

							case 32773:
								returnValue = ImageConstants.TiffEncodingPackBits;
								break;

							default:
								returnValue = $"Unknown ({value})";
								break;
						}
					}
				}

				return returnValue;
			}
		}

		/// <summary>
		/// Converts the byte array to a 64-bit integer.
		/// </summary>
		/// <param name="byteArray">
		/// The byte array.
		/// </param>
		/// <param name="offset">
		/// The offset.
		/// </param>
		/// <param name="length">
		/// The length.
		/// </param>
		/// <param name="byteOrdering">
		/// The byte ordering.
		/// </param>
		/// <returns>
		/// The 64-bit integer.
		/// </returns>
		private static long ConvertByteArrayToInt64(byte[] byteArray, int offset, int length, ByteOrdering byteOrdering)
		{
			long offsetPosition = 0;
			int start;
			int end;
			switch (byteOrdering)
			{
				case ByteOrdering.LittleEndian:
					start = length - 1;
					end = 0;
					while (start >= end)
					{
						offsetPosition += Convert.ToInt64(byteArray[start + offset])
						                  * (long)Math.Round(System.Math.Pow(256.0, start));
						start += -1;
					}

					break;

				case ByteOrdering.BigEndian:
					start = 0;
					end = length - 1;
					while (end <= start)
					{
						offsetPosition += Convert.ToInt64(byteArray[end + offset])
						                  * (long)Math.Round(System.Math.Pow(256.0, length - 1 - end));
						end++;
					}

					break;
			}

			return offsetPosition;
		}

		/// <summary>
		/// Gets the generic image format from the FreeImage value.
		/// </summary>
		/// <param name="freeImageFormat">
		/// The free image format.
		/// </param>
		/// <returns>
		/// The <see cref="ImageFormat"/> value.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Maintainability",
			"CA1502:AvoidExcessiveComplexity",
			Justification = "This is intentionally complex to avoid coupling the FreeImage library.")]
		private static ImageFormat GetImageFormat(FreeImageAPI.FREE_IMAGE_FORMAT freeImageFormat)
		{
			if (freeImageFormat == FREE_IMAGE_FORMAT.FIF_IFF)
			{
				return ImageFormat.Iff;
			}

			if (freeImageFormat == FREE_IMAGE_FORMAT.FIF_LBM)
			{
				return ImageFormat.Lbm;
			}

			switch (freeImageFormat)
			{
				case FREE_IMAGE_FORMAT.FIF_BMP:
					return ImageFormat.Bmp;

				case FREE_IMAGE_FORMAT.FIF_CUT:
					return ImageFormat.Cut;

				case FREE_IMAGE_FORMAT.FIF_DDS:
					return ImageFormat.Dds;

				case FREE_IMAGE_FORMAT.FIF_EXR:
					return ImageFormat.Exr;

				case FREE_IMAGE_FORMAT.FIF_FAXG3:
					return ImageFormat.Faxg3;

				case FREE_IMAGE_FORMAT.FIF_GIF:
					return ImageFormat.Gif;

				case FREE_IMAGE_FORMAT.FIF_HDR:
					return ImageFormat.Hdr;

				case FREE_IMAGE_FORMAT.FIF_ICO:
					return ImageFormat.Ico;

				case FREE_IMAGE_FORMAT.FIF_J2K:
					return ImageFormat.J2k;

				case FREE_IMAGE_FORMAT.FIF_JNG:
					return ImageFormat.Jng;

				case FREE_IMAGE_FORMAT.FIF_JP2:
					return ImageFormat.Jp2;

				case FREE_IMAGE_FORMAT.FIF_JPEG:
					return ImageFormat.Jpeg;

				case FREE_IMAGE_FORMAT.FIF_KOALA:
					return ImageFormat.Koala;

				case FREE_IMAGE_FORMAT.FIF_LBM:
					return ImageFormat.Lbm;

				case FREE_IMAGE_FORMAT.FIF_MNG:
					return ImageFormat.Mng;

				case FREE_IMAGE_FORMAT.FIF_PBM:
					return ImageFormat.Pbm;

				case FREE_IMAGE_FORMAT.FIF_PBMRAW:
					return ImageFormat.PbmRaw;

				case FREE_IMAGE_FORMAT.FIF_PCD:
					return ImageFormat.Pcd;

				case FREE_IMAGE_FORMAT.FIF_PCX:
					return ImageFormat.Pcx;

				case FREE_IMAGE_FORMAT.FIF_PFM:
					return ImageFormat.Pfm;

				case FREE_IMAGE_FORMAT.FIF_PGM:
					return ImageFormat.Pgm;

				case FREE_IMAGE_FORMAT.FIF_PGMRAW:
					return ImageFormat.PgmRaw;

				case FREE_IMAGE_FORMAT.FIF_PICT:
					return ImageFormat.Pict;

				case FREE_IMAGE_FORMAT.FIF_PNG:
					return ImageFormat.Png;

				case FREE_IMAGE_FORMAT.FIF_PPM:
					return ImageFormat.Ppm;

				case FREE_IMAGE_FORMAT.FIF_PPMRAW:
					return ImageFormat.PpmRaw;

				case FREE_IMAGE_FORMAT.FIF_PSD:
					return ImageFormat.Psd;

				case FREE_IMAGE_FORMAT.FIF_RAS:
					return ImageFormat.Ras;

				case FREE_IMAGE_FORMAT.FIF_RAW:
					return ImageFormat.Raw;

				case FREE_IMAGE_FORMAT.FIF_SGI:
					return ImageFormat.Sgi;

				case FREE_IMAGE_FORMAT.FIF_TARGA:
					return ImageFormat.Targa;

				case FREE_IMAGE_FORMAT.FIF_TIFF:
					return ImageFormat.Tiff;

				case FREE_IMAGE_FORMAT.FIF_UNKNOWN:
					return ImageFormat.Unknown;

				case FREE_IMAGE_FORMAT.FIF_WBMP:
					return ImageFormat.WBmp;

				case FREE_IMAGE_FORMAT.FIF_XBM:
					return ImageFormat.Xbm;

				case FREE_IMAGE_FORMAT.FIF_XPM:
					return ImageFormat.Xpm;

				default:
					return ImageFormat.Unknown;
			}
		}
	}
}
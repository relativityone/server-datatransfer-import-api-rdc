// ----------------------------------------------------------------------------
// <copyright file="ImageValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;

	using FreeImageAPI;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Represents a service class object to identify and validate images using the FreeImage native library.
	/// </summary>
	internal class ImageValidator : IImageValidator
	{
		/// <summary>
		/// Identifier value indicating that the data in the TIFF
		/// file is written in little-endian (Intel format) order.
		/// </summary>
		private const int LittleEndianFormatIdentifier = 0x49;

		/// <summary>
		/// Tag identifier of Image File Directory entry containing
		/// information about compression scheme used on the image data.
		/// </summary>
		private const int CompressionTagId = 259;

		/// <summary>
		/// Size of Image File Header in bytes.
		/// </summary>
		private const int ImageFileHeaderByteSize = 8;

		/// <summary>
		/// Size of single Image File Directory Entry in bytes.
		/// </summary>
		private const int ImageFileDirectoryEntryByteSize = 12;

		/// <summary>
		/// Value indicating that the data type of the information found in
		/// the Image File Directory Entry is 32-bit unsigned integer (Long).
		/// </summary>
		private const int LongDataType = 4;

		/// <summary>
		/// Value of Image File Directory Entry indicating that compression
		/// scheme used on the image data is CCITT Group 4 fax encoding.
		/// </summary>
		private const int SupportedEncoding = 4;

		/// <summary>
		/// Dictionary providing mapping between tiff encoding identifier and text description.
		/// </summary>
		private static readonly Dictionary<long, string> TiffEncodingDescription = new Dictionary<long, string>()
		{
			[1] = "none",
			[2] = "CCITT Group 3 1-Dimensional Modified Huffman",
			[3] = "CCITT Group 3 FAX",
			[4] = "CCITT Group 4 FAX",
			[5] = "LZW",
			[6] = "JPEG (old-style)",
			[7] = "JPEG (new-style)",
			[8] = "Deflate compression, using zlib data format",
			[32773] = "PackBits",
		};

		private readonly IByteArrayConverter byteArrayConverter;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageValidator"/> class.
		/// </summary>
		/// <param name="byteArrayConverter">Object converting byte array to integer.</param>
		public ImageValidator(IByteArrayConverter byteArrayConverter)
		{
			this.byteArrayConverter = byteArrayConverter;
		}

		/// <inheritdoc />
		public void Validate(string file)
		{
			file.ThrowIfNullOrEmpty(nameof(file));

			ImageFormat imageFormat = GetImageFormat(file);

			// Determine if the file is a supported image type
			switch (imageFormat)
			{
				case ImageFormat.Tiff:
					ValidateTiffImage(file);
					break;

				case ImageFormat.Jpeg:
					// This is OK.
					break;

				default:
					string formatMessage = string.Format(
						CultureInfo.CurrentCulture,
						Strings.ImageFormatNotSupportedError,
						file,
						imageFormat.ToString());
					throw new ImageValidationException(formatMessage);
			}
		}

		private static string GetTiffEncodingDescription(long encoding)
		{
			if (!TiffEncodingDescription.TryGetValue(encoding, out string description))
			{
				return $"Unknown ({encoding})";
			}

			return description;
		}

		private static ImageFormat ConvertToImageFormat(FreeImageAPI.FREE_IMAGE_FORMAT freeImageFormat)
		{
			switch (freeImageFormat)
			{
				case FREE_IMAGE_FORMAT.FIF_JPEG:
					return ImageFormat.Jpeg;

				case FREE_IMAGE_FORMAT.FIF_TIFF:
					return ImageFormat.Tiff;

				default:
					return ImageFormat.Unsupported;
			}
		}

		private static void ValidateTiffImageIsBlackAndWhite(string file)
		{
			FreeImageAPI.FIBITMAP image = FreeImageAPI.FreeImage.Load(FreeImageAPI.FREE_IMAGE_FORMAT.FIF_TIFF, file, 0);
			try
			{
				int bitDepth = Convert.ToInt32(FreeImageAPI.FreeImage.GetInfoHeaderEx(image).biBitCount);
				if (bitDepth > 1)
				{
					string message = string.Format(CultureInfo.CurrentCulture, Strings.TiffImageNotOneBitError, file, bitDepth);
					throw new ImageValidationException(message);
				}
			}
			catch (ImageValidationException)
			{
				throw;
			}
			catch (NullReferenceException e)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.TiffImageNotSupportedError, file);
				throw new ImageValidationException(message, e);
			}
			catch (Exception e)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageReadError, file);
				throw new ImageValidationException(message, e);
			}
			finally
			{
				if (image != FIBITMAP.Zero)
				{
					FreeImageAPI.FreeImage.Unload(image);
				}
			}
		}

		private static ImageFormat GetImageFormat(string file)
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
			if (!fileInfo.Exists)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageFileNotFoundError, file);
				throw new FileNotFoundException(message, file);
			}

			if (fileInfo.Length == 0)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.ImageZeroBytesError, file);
				throw new ImageValidationException(message);
			}

			FreeImageAPI.FREE_IMAGE_FORMAT freeImageFormat = FreeImageAPI.FreeImage.GetFileType(file, 0);
			return ConvertToImageFormat(freeImageFormat);
		}

		private void ValidateTiffImage(string file)
		{
			ValidateTiffImageIsBlackAndWhite(file);
			this.ValidateTiffImageHasGroup4FaxEncoding(file);
		}

		private void ValidateTiffImageHasGroup4FaxEncoding(string file)
		{
			long encoding = GetTiffEncoding(file);

			if (encoding != SupportedEncoding)
			{
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.TiffEncodingNotSupportedError,
					file,
					GetTiffEncodingDescription(encoding),
					GetTiffEncodingDescription(SupportedEncoding));
				throw new ImageValidationException(message);
			}
		}

		private long GetTiffEncoding(string file)
		{
			byte[] buffer = new byte[12];
			using (System.IO.FileStream fileStream = new System.IO.FileStream(
				file,
				System.IO.FileMode.Open,
				System.IO.FileAccess.Read))
			{
				// Read the Image File Header to determine where the Image File Directory record is located.
				fileStream.Read(buffer, 0, ImageFileHeaderByteSize);
				ByteOrdering byteOrdering = buffer[0] == LittleEndianFormatIdentifier ? ByteOrdering.LittleEndian : ByteOrdering.BigEndian;

				long offsetToFirstImageFileDirectory = this.byteArrayConverter.ToInt64(buffer, 4, 4, byteOrdering);
				fileStream.Position = offsetToFirstImageFileDirectory;

				// Determine the number of Image File Directory entries
				fileStream.Read(buffer, 0, 2);
				long numberOfImageFileDirectoryEntries = this.byteArrayConverter.ToInt64(buffer, 0, 2, byteOrdering);

				// Check to see if this is a multi - page TIFF (contains more than one Image File Directory).
				long currentPosition = fileStream.Position;
				fileStream.Position += numberOfImageFileDirectoryEntries * ImageFileDirectoryEntryByteSize;
				fileStream.Read(buffer, 0, 4);

				long offsetToNextImageFileDirectory = this.byteArrayConverter.ToInt64(buffer, 0, 4, byteOrdering);
				if (offsetToNextImageFileDirectory > 0)
				{
					string message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TiffMultiPageNotSupportedError,
						file);
					throw new ImageValidationException(message);
				}

				fileStream.Position = currentPosition;

				// Determine the compression type used by looping through the Image File Directory entries
				for (int i = 0; i < numberOfImageFileDirectoryEntries; i++)
				{
					fileStream.Read(buffer, 0, ImageFileDirectoryEntryByteSize);
					long tagId = this.byteArrayConverter.ToInt64(buffer, 0, 2, byteOrdering);
					if (tagId == CompressionTagId)
					{
						// The type determines whether the value is stored as SHORT (2 bytes) or LONG (4 bytes). This makes a HUGE difference for Motorola byte ordering.
						long dataType = this.byteArrayConverter.ToInt64(buffer, 2, 2, byteOrdering);
						int valueSizeBytes = dataType == LongDataType ? 4 : 2;

						long tiffEncoding = this.byteArrayConverter.ToInt64(buffer, 8, valueSizeBytes, byteOrdering);
						return tiffEncoding;
					}
				}

				return -1;
			}
		}
	}
}
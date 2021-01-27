// -----------------------------------------------------------------------------------------------------
// <copyright file="RandomHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;

	using FizzWare.NBuilder;

	/// <summary>
	/// Represents static helper methods to create random test files and data.
	/// </summary>
	public static class RandomHelper
	{
		/// <summary>
		/// The random instance.
		/// </summary>
		private static readonly Random RandomInstance = new Random();

		/// <summary>
		/// The random generator instance.
		/// </summary>
		private static readonly RandomGenerator RandomGeneratorInstance = new RandomGenerator(42);

		/// <summary>
		/// Gets random file name.
		/// </summary>
		/// <returns>File name.</returns>
		private static string GetRandomFileName => Path.GetRandomFileName();

		/// <summary>
		/// Creates a new binary file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextBinaryFile(long minLength, long maxLength, string directory)
		{
			var fileName = "IXApi_TestFile_" + DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("D");
			return NextBinaryFile(minLength, maxLength, directory, fileName);
		}

		/// <summary>
		/// Creates a new binary file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextBinaryFile(long minLength, long maxLength, string directory, string fileName)
		{
			Directory.CreateDirectory(directory);

			var file = Path.Combine(directory, fileName);
			using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var length = RandomGeneratorInstance.Next(minLength, maxLength);
				fileStream.SetLength(length);
				return file;
			}
		}

		/// <summary>
		/// Gets the next random integer value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static int NextInt(int minValue, int maxValue)
		{
			return RandomInstance.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static double NextDouble(int minValue, int maxValue)
		{
			double value = NextInt(minValue, maxValue);
			return value;
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static decimal NextDecimal(int minValue, int maxValue)
		{
			decimal value = NextInt(minValue, maxValue);
			return value;
		}

		/// <summary>
		/// Creates a new random large text file for the specified file <paramref name="length"/>. This method should be used when the <paramref name="length"/> is greater than 1 MB.
		/// </summary>
		/// <param name="encoding">
		/// The encoding used to write the text file.
		/// </param>
		/// <param name="length">
		/// The text length.
		/// </param>
		/// <param name="file">
		/// The full path to the file to create.
		/// </param>
		public static void NextLargeTextFile(System.Text.Encoding encoding, long length, string file)
		{
			string directory = System.IO.Path.GetDirectoryName(file);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (FileStream fileStream = File.OpenWrite(file))
			{
				StreamWriter writer = new StreamWriter(fileStream, encoding);
				const int MaxPhraseLength = 1000;
				long remaining = length;
				while (remaining > 0)
				{
					string text = NextPhrase(
						remaining < MaxPhraseLength ? Convert.ToInt32(remaining) : MaxPhraseLength);
					writer.WriteLine(text);
					remaining -= text.Length;
				}

				writer.Flush();
			}
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <param name="file">
		/// The full path to the file to create.
		/// </param>
		public static void NextTextFile(int minValue, int maxValue, string file)
		{
			string directory = System.IO.Path.GetDirectoryName(file);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var text = NextString(minValue, maxValue);
			File.WriteAllText(file, text);
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="readOnly">
		/// Specify whether to set the file read-only attribute.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextTextFile(int minLength, int maxLength, string directory, bool readOnly)
		{
			var fileName = GetRandomFileName;
			var file = NextTextFile(minLength, maxLength, directory, fileName);
			if (!readOnly)
			{
				return file;
			}

			var fileAttributes = File.GetAttributes(file);
			File.SetAttributes(file, fileAttributes | FileAttributes.ReadOnly);
			return file;
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextTextFile(int minValue, int maxValue, string directory, string fileName)
		{
			checked
			{
				Directory.CreateDirectory(directory);

				if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
				{
					fileName = Path.ChangeExtension(fileName, ".txt");
				}

				var text = NextString(minValue, maxValue);
				var file = Path.Combine(directory, fileName);
				File.WriteAllText(file, text);
				return file;
			}
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <param name="file">
		/// The full path to the file to create.
		/// </param>
		/// <param name="encoding">
		/// File encoding.
		/// </param>
		public static void NextTextFile(int minValue, int maxValue, string file, System.Text.Encoding encoding)
		{
			string directory = System.IO.Path.GetDirectoryName(file);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var text = NextString(minValue, maxValue);
			File.WriteAllText(file, text, encoding);
		}

		/// <summary>
		/// Creates a new random pdf file with text content whose text size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		/// This method should be used later in production import integration tests
		public static string NextPdfFile(int minLength, int maxLength, string directory)
		{
			var file = NextPdfFile(minLength, maxLength, directory, GetRandomFileName);

			return file;
		}

		/// <summary>
		///  Creates a new random pdf file with text content whose text size is between <paramref name="minTextLength"/> and <paramref name="maxTextLength"/>.
		/// </summary>
		/// <param name="minTextLength">
		/// The minimum value.
		/// </param>
		/// <param name="maxTextLength">
		/// The maximum value.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextPdfFile(int minTextLength, int maxTextLength, string directory, string fileName)
		{
			Directory.CreateDirectory(directory);

			if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
			{
				fileName = Path.ChangeExtension(fileName, ".pdf");
			}

			string text = NextString(minTextLength, maxTextLength);
			string file = Path.Combine(directory, fileName);

			using (FileStream fs = new FileStream(file, FileMode.Create))
			using (var document = new iTextSharp.text.Document())
			{
				iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);
				document.Open();
				document.Add(new iTextSharp.text.Paragraph(text));
			}

			return file;
		}

		/// <summary>
		/// Creates a new random image file.
		/// </summary>
		/// <param name="imageFormat">
		/// The image format. Jpeg and Tiff are supported.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="width">
		/// The image width.
		/// </param>
		/// <param name="height">
		/// The image height.
		/// </param>
		/// <returns>
		/// Information about image file.
		/// </returns>
		public static FileInfo NextImageFile(Relativity.DataExchange.Media.ImageFormat imageFormat, string directory, int width, int height)
		{
			var fileName = GetRandomFileName;
			return NextImageFile(imageFormat, directory, width, height, fileName);
		}

		/// <summary>
		/// Creates a new random image file.
		/// </summary>
		/// <param name="imageFormat">
		/// The image format. Jpeg and Tiff are supported.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="width">
		/// The image width.
		/// </param>
		/// <param name="height">
		/// The image height.
		/// </param>
		/// <param name="fileName">
		/// File name.
		/// </param>
		/// <returns>
		/// Information about image file.
		/// </returns>
		public static FileInfo NextImageFile(Relativity.DataExchange.Media.ImageFormat imageFormat, string directory, int width, int height, string fileName)
		{
			switch (imageFormat)
			{
				case Relativity.DataExchange.Media.ImageFormat.Jpeg:
					return NextJpegFile(directory, fileName, width, height);
				case Relativity.DataExchange.Media.ImageFormat.Tiff:
					return NextTiffFile(directory, fileName, width, height);
				default:
					throw new InvalidOperationException("Not supported image format");
			}
		}

		/// <summary>
		///  Creates a new random jpg file whose resolution is defined by <paramref name="width"/> and <paramref name="height"/>.
		/// </summary>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <param name="width">
		/// The image width.
		/// </param>
		/// <param name="height">
		/// The image height.
		/// </param>
		/// <returns>
		/// Information about image file.
		/// </returns>
		public static FileInfo NextJpegFile(string directory, string fileName, int width, int height)
		{
			Directory.CreateDirectory(directory);

			if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
			{
				fileName = Path.ChangeExtension(fileName, ".jpeg");
			}

			var file = Path.Combine(directory, fileName);
			using (var bitmap = new Bitmap(width, height))
			{
				// adding anything to bitmap
				var graphics = Graphics.FromImage(bitmap);
				graphics.Clear(System.Drawing.Color.Orange);
				bitmap.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			return new FileInfo(file);
		}

		/// <summary>
		///  Creates a new random tiff file whose resolution is defined by <paramref name="width"/> and <paramref name="height"/>.
		/// </summary>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <param name="width">
		/// The image width.
		/// </param>
		/// <param name="height">
		/// The image height.
		/// </param>
		/// <returns>
		/// Information about image file.
		/// </returns>
		public static FileInfo NextTiffFile(string directory, string fileName, int width, int height)
		{
			Directory.CreateDirectory(directory);

			if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
			{
				fileName = Path.ChangeExtension(fileName, ".tiff");
			}

			var file = Path.Combine(directory, fileName);
			using (var bitmap = new Bitmap(width, height))
			using (var graphics = Graphics.FromImage(bitmap))
			{
				// adding anything to bitmap
				graphics.Clear(System.Drawing.Color.Black);
				graphics.FillEllipse(System.Drawing.Brushes.White, width / 2, height / 2, width / 4, height / 4);

				// there is some special logic as Relativity requires Tiff Group4 compression
				ImageCodecInfo encoder = GetEncoder("image/tiff");
				var myEncoder = Encoder.Compression;
				using (var encoderParameters = new EncoderParameters(1))
				{
					var encoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionCCITT4);
					encoderParameters.Param[0] = encoderParameter;

					bitmap.Save(file, encoder, encoderParameters);
				}
			}

			return new FileInfo(file);
		}

		/// <summary>
		/// Gets the next random boolean value.
		/// </summary>
		/// <returns>
		/// The random boolean value.
		/// </returns>
		public static bool NextBoolean()
		{
			return RandomInstance.NextDouble() >= 0.5;
		}

		/// <summary>
		/// Gets the next random phrase with the specified <paramref name="length"/>.
		/// </summary>
		/// <param name="length">
		/// The phrase length.
		/// </param>
		/// <returns>
		/// The random string value.
		/// </returns>
		public static string NextPhrase(int length)
		{
			return RandomGeneratorInstance.Phrase(length).PadRight(length);
		}

		/// <summary>
		/// Gets the next random string value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random string value.
		/// </returns>
		public static string NextString(int minValue, int maxValue)
		{
			return RandomGeneratorInstance.NextString(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random Uri.
		/// </summary>
		/// <returns>
		/// The random <see cref="Uri"/> instance.
		/// </returns>
		public static Uri NextUri()
		{
			const string availableChars =
				"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			using (var generator = new RNGCryptoServiceProvider())
			{
				var bytes = new byte[16];
				generator.GetBytes(bytes);
				var chars = bytes
					.Select(b => availableChars[b % availableChars.Length]);
				var token = new string(chars.ToArray());
				return new Uri($"https://www.{token}.com");
			}
		}

		/// <summary>
		/// Gets the next random enumeration value.
		/// </summary>
		/// <param name="enumType">
		/// The enumeration type.
		/// </param>
		/// <returns>
		/// The random enumeration value.
		/// </returns>
		public static int NextEnum(Type enumType)
		{
			Array enumValues = Enum.GetValues(enumType);
			int randomValue = (int)enumValues.GetValue(RandomInstance.Next(enumValues.Length));
			return randomValue;
		}

		/// <summary>
		/// Gets the next random integer value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static int NextInt32(int minValue, int maxValue)
		{
			return RandomInstance.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random long value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random long value.
		/// </returns>
		public static long NextInt64(long minValue, long maxValue)
		{
			long value = minValue + (long)(RandomInstance.NextDouble() * (maxValue - minValue));
			return value;
		}

		/// <summary>
		/// Gets a random list using the specified constraints.
		/// </summary>
		/// <param name="min">
		/// The minimum value.
		/// </param>
		/// <param name="max">
		/// The maximum value.
		/// </param>
		/// <param name="targetSum">
		/// The target sum.
		/// </param>
		/// <param name="totalNumbers">
		/// The total number of randomly generated numbers to create.
		/// </param>
		/// <returns>
		/// The numbers.
		/// </returns>
		public static IEnumerable<int> GetRandomNumbers(int min, int max, int targetSum, int totalNumbers)
		{
			var ret = new List<int>(totalNumbers);
			var random = new Random();
			var remainingSum = targetSum;
			for (var i = 1; i <= totalNumbers; i++)
			{
				var localMax = remainingSum - ((totalNumbers - i) * min);
				if (localMax > max)
				{
					localMax = max;
				}

				var localMin = remainingSum - ((totalNumbers - i) * max);
				if (localMin > min || localMin < 0)
				{
					localMin = min;
				}

				if (i + 1 <= totalNumbers)
				{
					var nextDigit = random.Next(localMin, localMax);
					ret.Add(nextDigit);
					remainingSum -= nextDigit;
				}
				else
				{
					ret.Add(remainingSum);
				}
			}

			return ret;
		}

		/// <summary>
		/// Returns a non-negative random integer. The values are skewed towards the minValue.
		/// </summary>
		/// <param name="random">The random instance.</param>
		/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
		/// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
		/// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
		public static int NextBiased(this Random random, int minValue, int maxValue)
		{
			if (random == null)
			{
				throw new ArgumentNullException(nameof(random));
			}

			double biased = random.NextDouble() * random.NextDouble();
			biased *= maxValue - minValue;
			biased += minValue;

			return (int)biased;
		}

		/// <summary>
		/// Gets a random index of a list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="random">The random instance.</param>
		/// <param name="list">The list instance.</param>
		/// <returns>Random index for the list.</returns>
		public static int NextIndex<T>(this Random random, IList<T> list)
		{
			if (random == null)
			{
				throw new ArgumentNullException(nameof(random));
			}

			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			return random.Next(list.Count);
		}

		/// <summary>
		/// Gets a random element of a list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="random">The random instance.</param>
		/// <param name="list">The list instance.</param>
		/// <returns>Random element from the list.</returns>
		public static T NextElement<T>(this Random random, IList<T> list)
		{
			if (random == null)
			{
				throw new ArgumentNullException(nameof(random));
			}

			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			return list[random.NextIndex(list)];
		}

		private static ImageCodecInfo GetEncoder(string mimeType)
		{
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < encoders.Length; ++i)
			{
				if (encoders[i].MimeType == mimeType)
				{
					return encoders[i];
				}
			}

			return null;
		}
	}
}